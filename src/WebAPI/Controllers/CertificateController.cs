using System.Threading.Tasks;

using WebAPI.Utils;
using WebAPI.Controllers.DTOs;

using Application.Queries;

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

        [HttpPost("{id}/signature")]
        public async Task<IActionResult> SignCertificate(string id, [FromBody] SignCertificateDTO info) =>
            FromResult(await Mediator.Send(info.ToCommand(id)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCertificateDetails(string id) =>
            FromMaybeResult(await Mediator.Send(new GetCertificateByIdQuery(id)), "Certificate was not found by the given Id");

        #region Helpers

        private IActionResult Ok<T>(T result) =>
            base.Ok(Envelope.Ok(result));

        protected new IActionResult Ok() =>
            base.Ok(Envelope.Ok());

        private IActionResult Error(string errorMessage) =>
            BadRequest(Envelope.Error(errorMessage));

        private IActionResult FromResult<T>(Result<T> result)
        {
            if (result.IsFailure)
                return Error(result.Error);

            return Ok(result.Value);
        }

        protected IActionResult FromResult(Result result) =>
            result.IsSuccess ? Ok() : Error(result.Error);

        protected IActionResult FromMaybeResult<T>(Result<Maybe<T>> result, string notFoundMessage)
        {
            if (result.IsFailure)
                return Error(result.Error);

            if (result.Value.HasValue)
                return Ok(result.Value.Value);

            return NotFound(Envelope.Error(notFoundMessage));
        }

        #endregion
    }
}
