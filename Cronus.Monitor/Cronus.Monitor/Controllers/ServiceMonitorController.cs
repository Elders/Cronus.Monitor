using Microsoft.AspNetCore.Mvc;
using Cronus.Monitor.Models;
using System.Collections.Generic;
using static Cronus.Monitor.CronusMonitoringProjection;
using System.Linq;
using System.Collections.Concurrent;
using System;
using System.Collections;

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
        return Ok(monitorContainer.GetAllServiceStatus());
    }
}

public class ServiceStatus
{
    private ushort totalNodes = 0;
    private ushort operationalNodes = 0;

    public string BoundedContext { get; set; }

    public string Name { get; set; }

    public string Description { get; private set; }

    public string Status { get; private set; }

    public void ReportNodeOperational()
    {
        totalNodes++;
        operationalNodes++;

        CalculateStatus();
    }

    public void ReportNodeDown()
    {
        totalNodes++;

        CalculateStatus();
    }

    private void CalculateStatus()
    {
        if (totalNodes == operationalNodes && totalNodes > 0)
        {
            Status = "Operational";
            Description = $"{operationalNodes}/{totalNodes} nodes operational";
        }
        else if (totalNodes != operationalNodes && totalNodes > 0)
        {
            Status = "PartiallyDegraded";
            Description = $"{operationalNodes}/{totalNodes} nodes operational";
        }
        else
        {
            Status = "Down";
            Description = $"{operationalNodes}/{totalNodes} nodes operational";
        }
    }
}

