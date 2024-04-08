using Elders.Cronus;
using Elders.Cronus.Hosting.Heartbeat;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Cronus.Monitor;

[DataContract(Name = "454fad07-1b15-48b0-8d1d-667d5b1fd398")]
public class CronusMonitoringProjection : ITrigger,
    ISignalHandle<HeartbeatSignal>
{
    private readonly MonitorContainer monitorContainer;
    private readonly ILogger<CronusMonitoringProjection> logger;

    public CronusMonitoringProjection(MonitorContainer monitorContainer, ILogger<CronusMonitoringProjection> logger)
    {
        this.monitorContainer = monitorContainer;
        this.logger = logger;
    }

    public Task HandleAsync(HeartbeatSignal @event)
    {
        HeartbeatDto data = new HeartbeatDto(@event);
        monitorContainer.Add(data);

        return Task.CompletedTask;
    }
}

