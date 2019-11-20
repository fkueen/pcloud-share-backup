namespace PCloud.Backup
{
  public class BackupConfig
  {
    public string PCloudCode { get; set; }
    public string BackupFolder { get; } = "data";
    public string BackupLabel { get; set; } = "docker";
    public string BackupCronExpression { get; set; } = "0 0 0 * * ?";
  }
}

