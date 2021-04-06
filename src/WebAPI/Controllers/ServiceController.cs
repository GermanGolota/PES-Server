using Application.CQRS.Queries.Service.GetPesScore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/service")]
    public class ServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pesScore/{username}")]
        public async Task<ActionResult<int>> GetPesScore(string username, CancellationToken token)
        {
            var query = new GetPesScoreQuery
            {
                Username = username
            };
            int reponse = await _mediator.Send(query, token);
            return reponse;
        }
    }
}
