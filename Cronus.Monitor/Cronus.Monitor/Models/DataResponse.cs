using Elders.Cronus;
using System;
using System.Collections.Generic;

namespace Cronus.Monitor.Models
{
    internal class DataResponse : ResponseFromProjection<List<HeartBeatDto>>
    {
        public DataResponse(ReadResult<CronusMonitoringProjection> projResult)
        {
            Data = new List<HeartBeatDto>();

            if (projResult.IsSuccess == false || projResult.Data is null)
            {
                Errors.Add($"Unable to load {typeof(CronusMonitoringProjection).Name}");
                if (projResult.HasError)
                {
                    Errors.Add(projResult.Error);
                }
                return;
            }
        }
    }

    internal class HeartBeatDto
    {
        public HeartBeatDto(MonitorData data)
        {
            BoundedContext = data.BoundedContext;
            Tenants = data.Tenants;
            Timestamp = data.Timestamp;
            Tenant = data.Tenant;
            MachineName = data.MachineName;
        }

        public string Tenant { get; set; }

        public string BoundedContext { get; set; }

        public List<string> Tenants { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string MachineName { get; set; }
    }
}
