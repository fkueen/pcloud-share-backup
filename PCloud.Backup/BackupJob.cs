using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Collections.Generic;

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
        foreach (var backupFolder in _config.BackupFolder.Split(";", StringSplitOptions.RemoveEmptyEntries))
        {
          if (!Directory.Exists(backupFolder))
          {
            _logger.LogInformation($"Data directory '{backupFolder}' doesn't exist or empty. Skipping...");
            continue;
          }

          if (!Directory.EnumerateFileSystemEntries(backupFolder, _config.BackupPattern, SearchOption.AllDirectories).Any())
          {
            _logger.LogInformation($"Unable to find '{_config.BackupPattern}'. Skipping...");
            continue;
          }

          var backupFilenames = new List<string>();

          if (_config.BackupCompression)
          {
            var backupFilename = _zip.Execute(backupFolder, _config.BackupPattern);
            backupFilenames.Append(backupFilename);
          }
          else
          {
            backupFilenames.AddRange(Directory.EnumerateFileSystemEntries(backupFolder, _config.BackupPattern, SearchOption.AllDirectories));
          }

          foreach (var backupFilename in backupFilenames)
          {
            _logger.LogInformation($"Uploading {backupFilename}...");
            await _api.UploadToLinkAsync(backupFilename);
            File.Delete(backupFilename);
            _logger.LogInformation($"Done {backupFilename}");
          }
        }
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Job Error");
      }
    }
  }
}