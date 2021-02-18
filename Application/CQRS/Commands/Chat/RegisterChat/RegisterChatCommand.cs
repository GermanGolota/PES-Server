using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;

namespace Application.CQRS.Commands
{
    public class RegisterChatCommand : IRequest<bool>
    {
        [Required]
        public string ChatName { get; set; }
        public Guid AdminId { get; set; }
    }
}
