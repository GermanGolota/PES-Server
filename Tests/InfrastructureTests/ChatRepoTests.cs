﻿using Core;
using Core.Entities;
using Core.Exceptions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InfrastructureTests
{
    public class ChatRepoTests : IDisposable
    {
        private readonly ChatRepo _sut;
        private PESContext _context;

        private Guid adminId = new Guid("0bc3086a-0060-433f-b7ca-ad535e1c3465");

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
            //Act
            Func<Task> action = async () => await _sut.PromoteToAdmin(chatId, adminId);
            //Assert

            await Assert.ThrowsAsync<CannotPromoteAdminException>(action);
        }
        [Fact]
        public async Task PromoteToAdmin_ShouldNotWork_UserNotInChat()
        {
            //Arrange
            await SetupDbWithOneAdmin();
            Guid userNotInChatId = new Guid("fb71550b-5b74-40a0-b0ee-92c8c53ea8b8");

            //Act
            Func<Task> action = async () => await _sut.PromoteToAdmin(chatId, userNotInChatId);
            //Assert

            await Assert.ThrowsAsync<NoUserException>(action);
        }
        private static bool _beenSetup = false;
        private async Task SetupDbWithOneAdmin()
        {
            if (_beenSetup == false)
            {
                _beenSetup = true;

                User TestUser = new User
                {
                    Username = "testUser",
                    PasswordHash = "123",
                    UserId = adminId
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
                        UserId = adminId
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
}