using Elders.Cronus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Cronus.Monitor.Api;

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

        await cronusHost.StartAsync().ConfigureAwait(false);

        log.LogInformation("Service started!");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        log.LogInformation("Stopping service...");

        await cronusHost.StopAsync().ConfigureAwait(false); ;

        log.LogInformation("Service stopped");
    }
}
