using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Mars.Retranslator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mars.Retranslator.Controllers
{
    [ApiController]
    [Route("mcc")]
    public class MccController : ControllerBase
    {
        private readonly RetranslatorService _service;

        public MccController(RetranslatorService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("instances")]
        public async Task<IActionResult> GetInstancesAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetInstancesAsync(Request.GetAppKey(), cancellationToken);
            return Ok(result);
        }
    }
}
