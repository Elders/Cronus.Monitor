using Microsoft.AspNetCore.Mvc;
using Cronus.Monitor.Models;
using System.Collections.Generic;
using static Cronus.Monitor.CronusMonitoringProjection;
using System.Linq;
using System.Collections.Concurrent;
using System;

namespace Cronus.Monitor.Controllers;

[Route("Monitor")]
public class ServiceMonitorController : ControllerBase
{
    private readonly MonitorContainer monitorContainer;

    public ServiceMonitorController(MonitorContainer monitorContainer)
    {
        this.monitorContainer = monitorContainer;
    }

    [HttpGet("GetLiveServices")]
    public IActionResult GetBCServicesAsync()
    {
        List<string> result = new List<string>();
        ICollection<LimitedConcurrentQueue<HeartbeatDto>> services = monitorContainer.HeartBeats.Values;
        foreach (var service in services)
        {
            result.Add(service.Select(x => x.BoundedContext).FirstOrDefault());
        }
        return Ok(result);
    }

    [HttpGet("GetTenants")]
    public IActionResult GetTenantsAsync()
    {
        List<string> result = new List<string>();
        //ICollection<LimitedConcurrentQueue<HeartbeatDto>> tenants = monitorContainer.heartBeats.Values;
        //foreach (var tenant in tenants)
        //{
        //    result.Add(tenant.Select(x => x.Tenant).FirstOrDefault());
        //}
        return Ok(result);
    }

    [HttpGet("Data")]
    public IActionResult GetMonitoringServicesAsync()
    {
        return Ok(new ResponseResult<ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>>>(monitorContainer.HeartBeats));
    }

    [HttpGet("Status")]
    public IActionResult GetStatusAsync()
    {
        List<ServiceStatus> result = new List<ServiceStatus>();
        foreach (var service in monitorContainer.GetMonitoredServices())
        {
            var status = new ServiceStatus
            {
                BoundedContext = service,
                Name = service,
                Description = "description",
                Status = GetServiceStatus(service)
            };

            result.Add(status);
        }

        return Ok(result);
    }

    private string GetServiceStatus(string service)
    {
        bool hasHeartbeats = false;
        if (monitorContainer.HeartBeats.TryGetValue(service, out LimitedConcurrentQueue<HeartbeatDto> data))
        {
            hasHeartbeats = data.Where(x => x.Timestamp > DateTimeOffset.UtcNow.AddMinutes(-1)).Any();
        }

        return hasHeartbeats ? "operational" : "down";
    }
}

public class ServiceStatus
{
    public string BoundedContext { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Status { get; set; } // operational, down
}

