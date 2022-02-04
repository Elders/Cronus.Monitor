using Elders.Cronus;
using Elders.Cronus.Hosting.Heartbeat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cronus.Monitor
{
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


        public void Handle(HeartbeatSignal @event)
        {
            HeartbeatDto data = new HeartbeatDto(@event);
            monitorContainer.Add(data);
        }


        public class MonitorContainer
        {
            public ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>> heartBeats { get; private set; }

            public MonitorContainer()
            {
                heartBeats = new ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>>();
            }

            public void Add(HeartbeatDto heartbeatDto)
            {
                try
                {
                    LimitedConcurrentQueue<HeartbeatDto> queue;
                    if (heartBeats.TryGetValue(heartbeatDto.Id, out queue) == false)
                    {
                        queue = new LimitedConcurrentQueue<HeartbeatDto>(2);
                        queue.Enqueue(heartbeatDto);
                        heartBeats.TryAdd(heartbeatDto.Id, queue);
                    }
                    queue.Enqueue(heartbeatDto);


                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    public class HeartbeatDto
    {
        public HeartbeatDto(HeartbeatSignal @event)
        {
            BoundedContext = @event.BoundedContext;
            Tenants = @event.Tenants;
            Timestamp = @event.Timestamp;
            Tenant = @event.Tenant;
            MachineName = @event.MachineName;
            Environment = @event.EnvironmentConfig;
            Id = $"{Environment}_{BoundedContext}_{MachineName}";
        }

        public string Id { get; private set; }

        public string Tenant { get; private set; }

        public string BoundedContext { get; private set; }

        public List<string> Tenants { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public string MachineName { get; private set; }

        public string Environment { get; private set; }
    }
}

