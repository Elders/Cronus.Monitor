using Elders.Cronus;
using Elders.Cronus.Multitenancy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Cronus.Monitor.Api
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> log;
        private readonly ICronusHost cronusHost;

        public Worker(ILogger<Worker> log, ICronusHost cronusHost)
        {
            this.log = log;
            this.cronusHost = cronusHost;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            log.LogInformation("Starting service...");

            cronusHost.Start();

            log.LogInformation("Service started!");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Stopping service...");

            cronusHost.Stop();

            log.LogInformation("Service stopped");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
