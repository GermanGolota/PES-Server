using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core;

public class PESContext : DbContext
{
    public PESContext(DbContextOptions<PESContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<UserToChat> ChatUsers { get; set; }

    public DbSet<RefreshToken> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserToChat>().HasKey(o => new
        {
            o.ChatId,
            o.UserId
        });
    }
}