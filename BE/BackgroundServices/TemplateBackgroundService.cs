using Microsoft.EntityFrameworkCore;
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
            Func<Task> task = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var _dataContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>())
                {
                    var list_cm = await _dataContext.Comics.Where(m => m.deleted_at == null).Select(m => m.id).ToListAsync();
                    foreach (var cm_id in list_cm)
                    {
                        await _dataContext.Comics
                            .ExecuteUpdateAsync(m => m.SetProperty(p =>
                            p.chapter_count,
                            _dataContext.ComicChapters.Where(chap => chap.comic_id == cm_id && chap.deleted_at == null).Count()));
                    }
                }
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken); // Run every w
            }
        }
    }
}