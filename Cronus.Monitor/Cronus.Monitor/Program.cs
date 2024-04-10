using Elders.Pandora;
using Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Cronus.Monitor;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("APPLICATION", App.Name, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("pandora_application", App.Name, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("log_name", App.LogName, EnvironmentVariableTarget.Process);

        Start.WithStartupDiagnostics(App.Name, () => CreateHostBuilder(args).Build().Run());
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                    configApp.Add(new PandoraConsulConfigurationSource(Environment.GetEnvironmentVariable("CONSUL_ADDRESS", EnvironmentVariableTarget.Process)));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = false;
                })
                .UseSerilog(SerilogConfiguration.Configure);
}
