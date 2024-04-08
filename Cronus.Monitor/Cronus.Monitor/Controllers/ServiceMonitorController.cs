using Microsoft.AspNetCore.Mvc;
using Cronus.Monitor.Models;
using System.Collections.Generic;
using static Cronus.Monitor.CronusMonitoringProjection;
using System.Linq;
using System.Collections.Concurrent;

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
        ICollection<LimitedConcurrentQueue<HeartbeatDto>> services = monitorContainer.heartBeats.Values;
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
        return Ok(new ResponseResult<ConcurrentDictionary<string, LimitedConcurrentQueue<HeartbeatDto>>>(monitorContainer.heartBeats));
    }
}

