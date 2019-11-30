using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.DependencyInjection.Microsoft.Extensions;
using Serilog;
using Serilog.Filters;

namespace PCloud.Backup
{
  class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = new HostBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();
        })
        .ConfigureServices((hostContext, services) =>
        {
          services
            .Configure<ConsoleLifetimeOptions>(o => o.SuppressStatusMessages = true)
            .AddLogging()
            .AddQuartz()
            .AddSingleton<PCloudApi>()
            .AddSingleton<ZipService>()
            .AddSingleton<BackupConfig>(hostContext.Configuration.Get<BackupConfig>())
            .AddTransient<BackupJob>()
            .AddHostedService<BackupService>();
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
          loggerConfiguration
            .MinimumLevel.Information()
            .Filter.ByExcluding(Matching.FromSource("Quartz"))
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {SourceContext} {Message}{NewLine}{Exception}");
        });

      await builder.RunConsoleAsync();
    }
  }
}
