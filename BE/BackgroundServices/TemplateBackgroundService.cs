using TruyenCV.Models;

namespace TruyenCV.BackgroundServices
{
    public class TemplateBackgroundService : BackgroundService
    {
        private readonly ILogger<TemplateBackgroundService> _logger;
        private readonly AppDataContext _dataContext = null!;
        private readonly static object _lock = new();
        private readonly static Queue<Func<Task>> _tasks = new();
        public TemplateBackgroundService(ILogger<TemplateBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _dataContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>();
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
                //
                while (_tasks.Count > 0)
                {
                    lock (_lock)
                    {
                        if (_tasks.Count > 0)
                        {
                            task = _tasks.TryDequeue(out var dequeuedTask) ? dequeuedTask : null;
                        }
                    }
                    if (task != null)
                    {
                        try
                        {
                            await task();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing background task");
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Run every day
            }
        }
    }
}