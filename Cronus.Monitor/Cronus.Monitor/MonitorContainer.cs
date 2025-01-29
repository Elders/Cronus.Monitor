using Cronus.Monitor.Controllers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cronus.Monitor;

public class MonitorContainer
{
    public ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>> HeartBeats { get; private set; }

    public MonitorContainer()
    {
        HeartBeats = new ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>>();
    }

    public void Add(HeartbeatDto heartbeatDto)
    {
        try
        {
            LimitedConcurrentQueue<HeartbeatDto> queue;
            if (HeartBeats.TryGetValue(heartbeatDto.Id, out queue) == false)
            {
                queue = new LimitedConcurrentQueue<HeartbeatDto>(2);
                queue.Enqueue(heartbeatDto);
                HeartBeats.TryAdd(heartbeatDto.Id, queue);
            }
            else
            {
                queue.Enqueue(heartbeatDto);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static TimeSpan OperationalWindowDelay = TimeSpan.FromMinutes(1);

    public List<ServiceStatus> GetAllServiceStatus()
    {
        var serviceStatusList = new List<ServiceStatus>();

        foreach (var kvp in HeartBeats)
        {
            var serviceId = kvp.Key;
            var queue = kvp.Value;

            if (queue == null || queue.Count == 0)
            {
                serviceStatusList.Add(new ServiceStatus
                {
                    BoundedContext = queue.FirstOrDefault()?.BoundedContext,
                    Name = serviceId
                });
                continue;
            }

            var service = serviceStatusList.Where(s => s.BoundedContext == queue.First().BoundedContext).SingleOrDefault();
            if (service is null)
            {
                service = new ServiceStatus()
                {
                    BoundedContext = queue.First().BoundedContext,
                    Name = queue.First().BoundedContext,
                };
                serviceStatusList.Add(service);
            }

            bool isNodeOperational = queue.Where(hb => DateTimeOffset.UtcNow - hb.Timestamp < OperationalWindowDelay).Any();
            if (isNodeOperational)
            {
                service.ReportNodeOperational();
            }
            else
            {
                service.ReportNodeDown();
            }
        }

        return serviceStatusList;
    }
}
