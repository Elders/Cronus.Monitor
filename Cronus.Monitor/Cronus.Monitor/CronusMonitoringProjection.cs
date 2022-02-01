using Elders.Cronus;
using Elders.Cronus.Hosting.Heartbeat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cronus.Monitor
{
    [DataContract(Name = "454fad07-1b15-48b0-8d1d-667d5b1fd398")]
    public class CronusMonitoringProjection : ITrigger,
        ISignalHandle<HeartbeatEvent>
    {

        private readonly MonitorContainer monitorContainer;
        public readonly ILogger<CronusMonitoringProjection> logger;

        public CronusMonitoringProjection(MonitorContainer monitorContainer, ILogger<CronusMonitoringProjection> logger)
        {
            this.monitorContainer = monitorContainer;
            this.logger = logger;
        }


        public void Handle(HeartbeatEvent @event)
        {
            //List<MonitorData> monitorData = State.Data.FindAll(o => o.MachineName.Equals(@event.ServiceId));

            MonitorData data = new MonitorData(@event);
            monitorContainer.Add(data);
        }


        public class MonitorContainer
        {
            public MonitorContainer()
            {
                //var Qdata = new Queue<MonitorData>(10);

                Data = new List<MonitorData>();

            }

            public List<MonitorData> Data { get; private set; }

            public void Add(MonitorData sample)
            {
                Data.Add(sample);
            }
        }
    }

    public class MonitorData
    {
        public MonitorData(HeartbeatEvent @event)
        {
            BoundedContext = @event.BoundedContext;
            Tenants = @event.Tenants;
            Timestamp = @event.Timestamp;
            Tenant = @event.Tenant;
            MachineName = @event.MachineName;
        }

        public string Tenant { get; private set; }

        public string BoundedContext { get; private set; }

        public List<string> Tenants { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public string MachineName { get; private set; }
    }
}

