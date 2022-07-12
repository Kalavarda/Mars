using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
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
        [Route("resolveCommands")]
        public async Task<IActionResult> ResolveCommandsAsync(CancellationToken cancellationToken)
        {
            var result = await _service.ResolveCommandsAsync(GetMachineName(), Request.GetAppKey(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("confirmResolve")]
        public async Task<IActionResult> ConfirmResolveAsync(uint id, CancellationToken cancellationToken)
        {
            await _service.ConfirmResolveAsync(id, Request.GetAppKey(), cancellationToken);
            return Ok();
        }

        [HttpPost]
        [Route("submitExecResult")]
        public async Task<IActionResult> SubmitExecResultAsync(uint id, [FromBody]string data, CancellationToken cancellationToken)
        {
            var result = await _service.SubmitExecResultAsync(id, Convert.FromBase64String(data), Request.GetAppKey(), cancellationToken);
            return Ok(result);
        }

        private string GetMachineName()
        {
            if (Request.Headers.ContainsKey(nameof(Environment.MachineName)))
                return Request.Headers[nameof(Environment.MachineName)];
            return null;
        }
    }
}
