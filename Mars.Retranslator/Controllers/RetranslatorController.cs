using System;
using System.Threading;
using System.Threading.Tasks;
using Mars.Retranslator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mars.Retranslator.Controllers
{
    [ApiController]
    [Route("retranslator")]
    public class RetranslatorController: ControllerBase
    {
        private readonly RetranslatorService _service;

        public RetranslatorController(RetranslatorService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [Route("resolveCommand")]
        public async Task<IActionResult> ResolveCommandAsync(CancellationToken cancellationToken)
        {
            var result = await _service.ResolveCommandAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("receiveCommand")]
        public async Task<IActionResult> ReceiveCommandAsync(uint id, [FromBody]string data, CancellationToken cancellationToken)
        {
            var result = await _service.ReceiveCommandAsync(id, Convert.FromBase64String(data), cancellationToken);
            return Ok(result);
        }
    }
}
