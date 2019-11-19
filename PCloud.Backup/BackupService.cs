using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Matchers;

namespace PCloud.Backup
{
  public class BackupService : BackgroundService
  {
    private readonly ILogger _logger;
    private readonly IOptions<BackupConfig> _config;
    private readonly ISchedulerFactory _schedulerFactory;

    public BackupService(ILogger<BackupService> logger, IOptions<BackupConfig> config, ISchedulerFactory schedulerFactory)
    {
      _logger = logger;
      _config = config;
      _schedulerFactory = schedulerFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      try
      {
        _logger.LogInformation($"Starting up with cron expression {_config.Value.BackupCronExpression}...");

        var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
        await scheduler.Start(stoppingToken);

        await scheduler.ScheduleJob(
          JobBuilder.Create<BackupJob>().Build(),
          TriggerBuilder.Create().WithCronSchedule(_config.Value.BackupCronExpression).StartNow().Build(),
          stoppingToken);
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Daemon start error");
      }
    }

    public async override Task StopAsync(CancellationToken stoppingToken)
    {
      try
      {
        var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);
        foreach (var jobKey in await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
          await scheduler.Interrupt(jobKey, stoppingToken);

        await scheduler.Shutdown(true, stoppingToken);
        await base.StopAsync(stoppingToken);
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Daemon stop error");
      }
    }
  }
}
