﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace PCloud.Backup
{
  public class ZipService
  {
    private readonly ILogger _logger;
    private readonly BackupConfig _config;

    public ZipService(ILogger<ZipService> logger, BackupConfig config)
    {
      _logger = logger;
      _config = config;
    }

    public string Execute(string backupFolder, string backupPattern)
    {
      var backupFilename = $"{_config.SenderName}-{Path.GetFileName(backupFolder)}-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}.tar.gz";

      _logger.LogInformation($"Compressing {backupFilename}...");

      using (Stream stream = File.OpenWrite(backupFilename))
      using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, new WriterOptions(CompressionType.GZip)))
      {
        writer.WriteAll(backupFolder, backupPattern, SearchOption.AllDirectories);
      }

      return backupFilename;
    }
  }
}
