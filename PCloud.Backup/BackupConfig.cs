using System;

namespace PCloud.Backup
{
  public class BackupConfig
  {
    public string PCloudCode { get; set; }
    public string BackupFolder { get; set; } = "data";
    public string BackupPattern { get; set; } = "*";
    public string SenderName { get; set; } = "docker";
    public string BackupCronExpression { get; set; } = "0 0 0 * * ?";
    public bool BackupCompression { get; set; } = true;
    public int UploadBufferSize { get; set; } = 5242880;
    public TimeSpan UploadTimeout { get; set; } = TimeSpan.FromMinutes(10);
  }
}

