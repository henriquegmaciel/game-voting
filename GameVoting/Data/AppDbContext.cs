using GameVoting.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameVoting.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<GameSession>()
            .HasOne(s => s.Game)
            .WithMany(g => g.Sessions)
            .HasForeignKey(s => s.GameId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
