using Application.DTOs;
using Application.DTOs.Chat;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IChatRepo
    {
        Task CreateChat(Chat chat, User admin);
        Task DeleteChat(Guid chatId);
        Task<Chat> GetChatById(Guid chatId);
        Task<ChatDisplayModel> GetChatModel(Guid chatId);
        Task<List<ChatInfoModel>> GetChats(ChatSelectionOptions options);

        Task RemoveUserFromChat(Guid chatId, Guid userId);
        Task<List<Guid>> GetAdminsOfChat(Guid chatId);
        Task AddUser(Guid chatId, Guid userId, string chatPassword);
        Task PromoteToAdmin(Guid chatId, Guid userId);
        Task<MemberModel> GetMember(Guid chatId, Guid userId);
        Task<List<ChatMemberModel>> GetChatMembers(Guid chatId);
        Task<List<ChatMemberModelAdmin>> GetChatMembersAdmin(Guid chatId);
    }
}
