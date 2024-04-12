using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            queue.Enqueue(heartbeatDto);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IEnumerable<string> GetMonitoredServices()
    {
        return HeartBeats.Keys;
    }
}

