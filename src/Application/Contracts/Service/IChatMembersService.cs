using Application.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Contracts.Service
{
    public interface IChatMembersService
    {
        Task RemoveUserFromChat(Guid chatId, Guid userId);
        Task<List<Guid>> GetAdminsOfChat(Guid chatId);
        Task AddUser(Guid chatId, Guid userId, string chatPassword);
        Task PromoteToAdmin(Guid chatId, Guid userId);
        Task<MemberModel> GetMember(Guid chatId, Guid userId);
        Task<List<ChatMemberModel>> GetChatMembers(Guid chatId);
        Task<List<ChatMemberModelAdmin>> GetChatMembersAdmin(Guid chatId);
        Task RemoveUserFromAllChats(Guid userId);
    }
}
