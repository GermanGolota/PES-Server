using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Service;
using Application.DTOs;
using Application.DTOs.Chat;
using Core.Exceptions;
using MediatR;

namespace Application.CQRS.Commands
{
    public class UploadImageHandler : IRequestHandler<UploadImageCommand, CommandResponse>
    {
        private readonly IChatImageService _imageService;
        private readonly IChatMembersService _membersService;

        public UploadImageHandler(IChatImageService imageService, IChatMembersService membersService)
        {
            _imageService = imageService;
            _membersService = membersService;
        }

        public async Task<CommandResponse> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            CommandResponse response;
            try
            {
                var admins = await _membersService.GetAdminsOfChat(request.ChatId);
                if (admins.Contains(request.RequesterId))
                {
                    var update = new ChatImageUpdateRequest
                    {
                        FileExtension = request.FileExtension,
                        ImageStream = request.ImageStream
                    };
                    await _imageService.UpdateChatsImage(request.ChatId, update);

                    response = CommandResponse.CreateSuccessfull("Sucessfully updated chats image");
                }
                else
                {
                    response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.Unathorized);
                }
            }
            catch (Exception exc)
            {
                if (exc is ExpectedException)
                {
                    response = CommandResponse.CreateUnsuccessfull(exc.Message);
                }
                else
                {
                    response = CommandResponse.CreateUnsuccessfull(ExceptionMessages.ServerError);
                }
            }
            finally
            {
                request.ImageStream.Close();
            }
            return response;
        }
    }
}
