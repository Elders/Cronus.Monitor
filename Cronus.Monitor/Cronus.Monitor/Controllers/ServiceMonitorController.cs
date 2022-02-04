using Microsoft.AspNetCore.Mvc;
using Cronus.Monitor.Models;
using System.Collections.Generic;
using static Cronus.Monitor.CronusMonitoringProjection;

namespace Cronus.Monitor.Controllers
{
    [Route("Monitor")]
    public class ServiceMonitorController : ControllerBase
    {
        private readonly MonitorContainer monitorContainer;

        public ServiceMonitorController(MonitorContainer monitorContainer)
        {
            this.monitorContainer = monitorContainer;
        }

        [HttpGet("Data")]
        public IActionResult GetMonitoringServicesAsync()
        {

            return Ok(new ResponseResult<Queue<HeartbeatDto>>(monitorContainer.heartBeats));
        }
    }
}

