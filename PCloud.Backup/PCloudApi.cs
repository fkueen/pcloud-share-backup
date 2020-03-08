using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Flurl;
using Flurl.Http;

namespace PCloud.Backup
{
  public class PCloudApi
  {
    private readonly ILogger _logger;
    private readonly BackupConfig _config;

    public PCloudApi(ILogger<PCloudApi> logger, BackupConfig config)
    {
      _logger = logger;
      _config = config;
    }

    public async Task<PCloudResponse> UploadToLinkAsync(string fileName)
    {
      using (var file = File.OpenRead(fileName))
      {
        var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(file, _config.UploadBufferSize);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
          Name = "\"file\"",
          FileName = "\"" + fileName + "\""
        };
        content.Add(fileContent);

        return await new Url($"https://api.pcloud.com/uploadtolink")
          .SetQueryParams(new { code = _config.PCloudCode, names = _config.SenderName })
          .WithTimeout(_config.UploadTimeout)
          .PostAsync(content)
          .ReceiveJson<PCloudResponse>();
      }
    }

  }
}