using Darts_Score_Management.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Darts_Score_Management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<Leg> Legs { get; set; }
        public DbSet<Turn> Turns { get; set; }
        public DbSet<Throw> Throws { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Pretty-print JSON
                PropertyNameCaseInsensitive = true, // Case-insensitive property matching
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Ignore null values
            };
            // Player configuration
            modelBuilder.Entity<Player>()
                .HasMany(p => p.GamePlayers)
                .WithOne(gp => gp.Player)
                .HasForeignKey(gp => gp.PlayerId);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Turns)
                .WithOne(t => t.Player)
                .HasForeignKey(t => t.PlayerId);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.WonLegs)
                .WithOne(l => l.Winner)
                .HasForeignKey(l => l.WinnerPlayerId)
                .IsRequired(false);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.WonSets)
                .WithOne(s => s.Winner)
                .HasForeignKey(s => s.WinnerPlayerId)
                .IsRequired(false);

            // Game configuration
            modelBuilder.Entity<Game>()
                .HasMany(g => g.GamePlayers)
                .WithOne(gp => gp.Game)
                .HasForeignKey(gp => gp.GameId);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Sets)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameId);

            // Serialize GameSettings as JSON
            modelBuilder.Entity<Game>()
                .Property(g => g.Settings)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<GameSettings>(v, options)
                );

            // GamePlayer configuration
            modelBuilder.Entity<GamePlayer>()
                .HasMany(gp => gp.Statistics)
                .WithOne(s => s.GamePlayer)
                .HasForeignKey(s => s.GamePlayerId);

            // Set configuration
            modelBuilder.Entity<Set>()
                .HasMany(s => s.Legs)
                .WithOne(l => l.Set)
                .HasForeignKey(l => l.SetId);

            // Leg configuration
            modelBuilder.Entity<Leg>()
                .HasMany(l => l.Turns)
                .WithOne(t => t.Leg)
                .HasForeignKey(t => t.LegId);

            // Turn configuration
            modelBuilder.Entity<Turn>()
                .HasMany(t => t.Throws)
                .WithOne(th => th.Turn)
                .HasForeignKey(th => th.TurnId);

            // Indexes for better performance
            modelBuilder.Entity<GamePlayer>()
                .HasIndex(gp => new { gp.GameId, gp.PlayerId })
                .IsUnique();

            modelBuilder.Entity<Set>()
                .HasIndex(s => new { s.GameId, s.SetNumber })
                .IsUnique();

            modelBuilder.Entity<Leg>()
                .HasIndex(l => new { l.SetId, l.LegNumber })
                .IsUnique();

            modelBuilder.Entity<Turn>()
                .HasIndex(t => new { t.LegId, t.PlayerId, t.TurnNumber })
                .IsUnique();

            modelBuilder.Entity<Throw>()
                .HasIndex(t => new { t.TurnId, t.ThrowNumber })
                .IsUnique();
        }

    }
}
