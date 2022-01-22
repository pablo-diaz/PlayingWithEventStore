using System.Threading.Tasks;

using WebAPI.Utils;
using WebAPI.Controllers.DTOs;

using MediatR;

using CSharpFunctionalExtensions;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateController : ControllerBase
    {
        private IMediator _mediator;
        private IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        [HttpPost]
        public async Task<IActionResult> RegisterNewCertificate([FromBody] RegisterNewCertificateDTO info) =>
            FromResult(await Mediator.Send(info.ToCommand()));

        #region Helpers
        private IActionResult Ok<T>(T result) =>
            base.Ok(Envelope.Ok(result));

        private IActionResult Error(string errorMessage) =>
            BadRequest(Envelope.Error(errorMessage));

        private IActionResult FromResult<T>(Result<T> result)
        {
            if (result.IsFailure)
                return Error(result.Error);

            return Ok(result.Value);
        }

        #endregion
    }
}
