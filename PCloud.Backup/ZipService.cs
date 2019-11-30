using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;

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

    public async Task ExecuteAsync(string filename, CancellationToken stoppingToken = default(CancellationToken))
    {
      await Task.CompletedTask;

      using (FileStream zipFileStream = File.Create(filename))
      using (var zipStream = new ZipOutputStream(zipFileStream))
      {
        zipStream.SetLevel(_config.ZipLevel);

        var files = Directory.GetFiles(_config.BackupFolder, _config.BackupPattern);

        foreach (var file in files)
        {
          var fileInfo = new FileInfo(file);
          var entryName = file;

          entryName = ZipEntry.CleanName(entryName);
          var newEntry = new ZipEntry(entryName);
          newEntry.DateTime = fileInfo.LastWriteTime;
          newEntry.Size = fileInfo.Length;
          zipStream.PutNextEntry(newEntry);

          var buffer = new byte[_config.ZipBufferSize];
          using (FileStream fileStream = File.OpenRead(file))
          {
            StreamUtils.Copy(fileStream, zipStream, buffer);
          }
          zipStream.CloseEntry();
        }
      }
    }
  }
}
