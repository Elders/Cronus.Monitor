using Elders.Cronus.Hosting.Heartbeat;
using System;
using System.Collections.Generic;

namespace Cronus.Monitor;

public class HeartbeatDto
{
    public HeartbeatDto(HeartbeatSignal @event)
    {
        BoundedContext = @event.BoundedContext;
        Tenants = @event.Tenants;
        Timestamp = @event.Timestamp;
        MachineName = @event.MachineName;
        Environment = @event.EnvironmentConfig;
        Id = $"{Environment}_{BoundedContext}_{MachineName}";
    }

    public string Id { get; private set; }

    public string BoundedContext { get; private set; }

    public List<string> Tenants { get; private set; }

    public DateTimeOffset Timestamp { get; private set; }

    public string MachineName { get; private set; }

    public string Environment { get; private set; }
}

