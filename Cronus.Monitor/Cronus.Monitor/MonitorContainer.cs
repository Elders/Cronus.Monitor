using System;
using System.Collections.Concurrent;

namespace Cronus.Monitor;

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

