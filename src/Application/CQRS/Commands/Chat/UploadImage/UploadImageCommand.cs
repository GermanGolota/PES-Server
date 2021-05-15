using Application.DTOs;
using MediatR;
using System;
using System.IO;

namespace Application.CQRS.Commands
{
    public class UploadImageCommand : IRequest<CommandResponse>
    {
        public Guid RequesterId { get; set; }
        public Stream ImageStream { get; set; }
        public string FileExtension { get; set; }
        public Guid ChatId { get; set; }
    }
}
