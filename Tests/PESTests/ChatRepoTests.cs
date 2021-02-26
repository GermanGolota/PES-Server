using Core;
using Core.Entities;
using Core.Exceptions;
using Infrastructure.Authentication;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PESTests
{
    public class ChatRepoTests : IDisposable
    {
        private readonly ChatRepo _sut;
        private PESContext _context;

        private Guid userId = new Guid("0bc3086a-0060-433f-b7ca-ad535e1c3465");

        private Guid chatId = new Guid("b71ffcf6-fdae-4936-b0fe-d596a21f0d04");
        public ChatRepoTests()
        {
            var options = new DbContextOptionsBuilder<PESContext>()
            .UseInMemoryDatabase(databaseName: "PESDB")
            .Options;
            _context = new PESContext(options);
            _sut = new ChatRepo(_context);
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task PromoteToAdmin_ShouldNotWork_UserAlreadyAdmin()
        {
            //Arrange
            await SetupDbWithOneAdmin();
            var chat = _context.Chats.ToList();

            //Act
            Func<Task> action = async ()=>await _sut.PromoteToAdmin(chatId, userId);
            //Assert

            await Assert.ThrowsAsync<CannotPromoteAdminException>(action);
        }
        private async Task SetupDbWithOneAdmin()
        {
            User TestUser = new User
            {
                Username = "testUser",
                PasswordHash = "123",
                UserId = userId
            };


            Chat TestChat = new Chat
            {
                ChatName = "test",
                ChatPassword = "1",
                Admins = new List<AdminToChat>
                {
                    new AdminToChat
                    {
                        ChatId =chatId,
                        UserId = userId
                    }
                },
                ChatId = chatId
            };

            await _context.Users.AddAsync(TestUser);

            await _context.Chats.AddAsync(TestChat);

            await _context.SaveChangesAsync();
        }
    }
}
