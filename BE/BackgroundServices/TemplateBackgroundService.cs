using Microsoft.EntityFrameworkCore;
using Serilog;
using TruyenCV.Models;

namespace TruyenCV.BackgroundServices
{
    public class TemplateBackgroundService : BackgroundService
    {
        private readonly ILogger<TemplateBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly static object _lock = new();
        private readonly static Queue<Func<Task>> _tasks = new();

        public TemplateBackgroundService(ILogger<TemplateBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public static void EnqueueTask(Func<Task> task)
        {
            lock (_lock)
            {
                _tasks.Enqueue(task);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Func<Task> task = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var _dataContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>())
                {
                    var list_cm = await _dataContext.Comics
                        .AsNoTracking()
                        .Where(m => m.deleted_at == null)
                        .Select(m => m.id)
                        .ToListAsync(stoppingToken);

                    Log.Information($"Updating chapter_count for {list_cm.Count} comics");

                    foreach (var cm_id in list_cm)
                    {
                        // Phải tính count trước, EF Core không translate Count() trong SetProperty sang SQL đúng
                        var chapterCount = await _dataContext.ComicChapters
                            .AsNoTracking()
                            .Where(chap => chap.comic_id == cm_id && chap.deleted_at == null)
                            .CountAsync(stoppingToken);

                        await _dataContext.Comics
                            .Where(m => m.id == cm_id)
                            .ExecuteUpdateAsync(setters => setters.SetProperty(
                                p => p.chapter_count,
                                chapterCount),
                                cancellationToken: stoppingToken);

                        Log.Debug($"Comic {cm_id}: chapter_count = {chapterCount}");
                    }

                    Log.Information("Completed updating chapter_count for all comics");
                }
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken); // Run every week
            }
        }
    }
}