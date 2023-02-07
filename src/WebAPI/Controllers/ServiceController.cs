using System.Threading;
using System.Threading.Tasks;
using Application.CQRS.Queries.Service.GetPesScore;
using Application.DTOs.Service;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
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
    public async Task<ActionResult<PesScoreResultModel>> GetPesScore(string username, CancellationToken token)
    {
        GetPesScoreQuery query = new GetPesScoreQuery
        {
            Username = username
        };
        PesScoreResultModel reponse = await _mediator.Send(query, token);
        return reponse;
    }
}