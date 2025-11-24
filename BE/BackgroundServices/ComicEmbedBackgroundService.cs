using Microsoft.EntityFrameworkCore;
using Pgvector;
using TruyenCV.Models;
using TruyenCV.Services;

namespace TruyenCV.BackgroundServices
{
    public class ComicEmbedBackgroundService : BackgroundService
    {
        private readonly ILogger<ComicEmbedBackgroundService> _logger;
        private readonly ITextEmbeddingService _textEmbeddingService = null!;
        private readonly IServiceProvider _serviceProvider;
        private readonly static object _lock = new();
        private readonly static Queue<(int comicId, string content)> _tasks = new();
        public ComicEmbedBackgroundService(ILogger<ComicEmbedBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _textEmbeddingService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ITextEmbeddingService>();
        }
        public static void EnqueueTask((int comicId, string content) task)
        {
            lock (_lock)
            {
                _tasks.Enqueue(task);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var _dataContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>())
                {
                    (int comicId, string content)? task = null;
                    lock (_lock)
                    {
                        task = _tasks.TryDequeue(out var dequeuedTask) ? dequeuedTask : null;
                    }
                    if (task != null)
                    {
                        try
                        {
                            var embeddingVector = new Vector((await _textEmbeddingService.CreateEmbeddingAsync(task.Value.content))[0]);
                            await _dataContext.Comics.Where(c => c.id == task.Value.comicId).ExecuteUpdateAsync(
                                c => c.SetProperty(c => c.search_vector,
                                    c => embeddingVector
                                ),
                                cancellationToken: stoppingToken
                            );
                            await _dataContext.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing background task");
                        }
                    }
                    else
                    {
                        var ListEmpty = await _dataContext.Comics
                            .Where(c => c.search_vector == null && !c.deleted_at.HasValue)
                            .Take(10)
                            .ToListAsync(cancellationToken: stoppingToken);
                        if (ListEmpty.Count > 0)
                        {
                            List<long> ids = ListEmpty.Select(c => c.id).ToList();
                            float[][] embeddings = await _textEmbeddingService.CreateEmbeddingAsync(ListEmpty.Select(c => $"{c.name}, {c.description}").ToArray());
                            foreach (var (comic, embedding) in ListEmpty.Zip(embeddings))
                            {
                                var embeddingVector = new Vector(embedding);
                                comic.search_vector = embeddingVector;
                                _dataContext.Comics.Update(comic);
                            }
                            await _dataContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                    //slug fix
                    var listSlug = await _dataContext.Comics.Where(c => c.slug.Length < c.name.Length - 5).ToListAsync();
                    listSlug.ForEach(c => c.slug = c.name.ToSlug());
                    _dataContext.Comics.UpdateRange(listSlug);
                    await _dataContext.SaveChangesAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Run every minute
                }
            }
        }
    }
}