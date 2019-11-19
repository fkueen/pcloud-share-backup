using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.IO.Compression;

namespace PCloud.Backup
{
  [DisallowConcurrentExecution]
  public class BackupJob : IJob
  {
    private readonly ILogger _logger;
    private readonly IOptions<BackupConfig> _config;
    private readonly PCloudApi _api;

    public BackupJob(ILogger<BackupJob> logger, IOptions<BackupConfig> config, PCloudApi api)
    {
      _logger = logger;
      _config = config;
      _api = api;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      try
      {
        if (!Directory.Exists(_config.Value.BackupFolder) || !Directory.EnumerateFileSystemEntries(_config.Value.BackupFolder).Any())
        {
          _logger.LogInformation($"Data directory doesn't exist or empty. Skipping...");
          return;
        }

        var randomName = Path.GetRandomFileName();
        var backupFilename = $"{_config.Value.BackupLabel}-{DateTime.Now:yyyyMMddHHmmss}.zip";
        ZipFile.CreateFromDirectory(_config.Value.BackupFolder, backupFilename, CompressionLevel.Optimal, true);

        await _api.UploadToLinkAsync(backupFilename, _config.Value.PCloudCode, _config.Value.BackupLabel);
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