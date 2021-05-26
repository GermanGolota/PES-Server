using System.Threading;
using System.Threading.Tasks;
using Application.Contracts.Service;
using Application.DTOs.Chat;

namespace Application.CQRS.Commands
{
    public class UploadImageHandler : PesCommand<UploadImageCommand>
    {
        private readonly IChatImageService _imageService;
        private readonly IChatMembersService _membersService;

        public UploadImageHandler(IChatImageService imageService, IChatMembersService membersService)
        {
            _imageService = imageService;
            _membersService = membersService;
        }

        public override string SuccessMessage => "Sucessfully updated chats image";

        public override async Task<bool> Authorize(UploadImageCommand request, CancellationToken token)
        {
            var admins = await _membersService.GetAdminsOfChat(request.ChatId);
            return admins.Contains(request.RequesterId);
        }

        public override async Task Run(UploadImageCommand request, CancellationToken token)
        {
            var update = new ChatImageUpdateRequest
            {
                FileExtension = request.FileExtension,
                ImageStream = request.ImageStream
            };
            await _imageService.UpdateChatsImage(request.ChatId, update);
        }

        public override Task CleanUp(UploadImageCommand request, CancellationToken token)
        {
            request.ImageStream.Close();
            return Task.CompletedTask;
        }
    }
}
