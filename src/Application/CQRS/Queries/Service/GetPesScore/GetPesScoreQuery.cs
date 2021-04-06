using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Application.CQRS.Queries.Service.GetPesScore
{
    public class GetPesScoreQuery : IRequest<int>
    {
        public string Username { get; set; }
    }
}
