using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace PCloud.Backup
{
  [DisallowConcurrentExecution]
  public class BackupJob : IJob
  {
    private readonly ILogger _logger;
    private readonly BackupConfig _config;
    private readonly PCloudApi _api;
    private readonly ZipService _zip;

    public BackupJob(ILogger<BackupJob> logger, BackupConfig config, PCloudApi api, ZipService zip)
    {
      _logger = logger;
      _config = config;
      _api = api;
      _zip = zip;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      try
      {
        if (!Directory.Exists(_config.BackupFolder)
        || !Directory.EnumerateFileSystemEntries(_config.BackupFolder, _config.BackupPattern).Any())
        {
          _logger.LogInformation($"Data directory doesn't exist or empty. Skipping...");
          return;
        }

        var backupFilename = $"{_config.SenderName}-{DateTime.Now:yyyyMMddHHmmss}.zip";

        await _zip.ExecuteAsync(backupFilename, context.CancellationToken);
        await _api.UploadToLinkAsync(backupFilename);
        File.Delete(backupFilename);

        _logger.LogInformation($"Done {backupFilename}");
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Job Error");
      }
    }
  }
}