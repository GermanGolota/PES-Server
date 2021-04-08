using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Service;
using MediatR;

namespace Application.CQRS.Queries.Service.GetPesScore
{
    public class GetPesScoreQuery : IRequest<PesScoreResultModel>
    {
        public string Username { get; set; }
    }
}
