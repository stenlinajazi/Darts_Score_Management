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
        public DbSet<LegStats> LegStats { get; set; }
        public DbSet<SetStats> SetStats { get; set; }
        public DbSet<GameStats> GameStats { get; set; }

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
                .HasForeignKey(gp => gp.PlayerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes from Player to GamePlayer

            modelBuilder.Entity<Player>()
                .HasMany(p => p.Turns)
                .WithOne(t => t.Player)
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes from Player to Turn

            modelBuilder.Entity<Player>()
                .HasMany(p => p.WonLegs)
                .WithOne(l => l.Winner)
                .HasForeignKey(l => l.WinnerPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // Set to NULL on delete if optional

            modelBuilder.Entity<Player>()
                .HasMany(p => p.WonSets)
                .WithOne(s => s.Winner)
                .HasForeignKey(s => s.WinnerPlayerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Game configuration
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Id).ValueGeneratedOnAdd(); // Explicitly set as identity
                entity.HasMany(g => g.GamePlayers)
                      .WithOne(gp => gp.Game)
                      .HasForeignKey(gp => gp.GameId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete GamePlayers if Game is deleted
                entity.HasMany(g => g.Sets)
                      .WithOne(s => s.Game)
                      .HasForeignKey(s => s.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(g => g.GameStats)
                      .WithOne(gs => gs.Game)
                      .HasForeignKey(gs => gs.GameId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

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
                .HasForeignKey(s => s.GamePlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GamePlayer>()
                .HasMany(gp => gp.LegStats)
                .WithOne(ls => ls.GamePlayer)
                .HasForeignKey(ls => ls.GamePlayerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade from GamePlayer to LegStats
            modelBuilder.Entity<GamePlayer>()
                .HasMany(gp => gp.SetStats)
                .WithOne(ss => ss.GamePlayer)
                .HasForeignKey(ss => ss.GamePlayerId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade from GamePlayer to SetStats
            modelBuilder.Entity<GamePlayer>()
                .HasMany(gp => gp.GameStats)
                .WithOne(gs => gs.GamePlayer)
                .HasForeignKey(gs => gs.GamePlayerId)
                .OnDelete(DeleteBehavior.Cascade);


            // Set configuration
            modelBuilder.Entity<Set>()
                .HasMany(s => s.Legs)
                .WithOne(l => l.Set)
                .HasForeignKey(l => l.SetId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Set>()
                .HasMany(s => s.SetStats)
                .WithOne(ss => ss.Set)
                .HasForeignKey(ss => ss.SetId)
                .OnDelete(DeleteBehavior.NoAction);


            // Leg configuration
            modelBuilder.Entity<Leg>()
                .HasMany(l => l.Turns)
                .WithOne(t => t.Leg)
                .HasForeignKey(t => t.LegId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Leg>()
                .HasMany(l => l.LegStats)
                .WithOne(ls => ls.Leg)
                .HasForeignKey(ls => ls.LegId)
                .OnDelete(DeleteBehavior.NoAction);


            // Turn configuration
            modelBuilder.Entity<Turn>()
                .HasMany(t => t.Throws)
                .WithOne(th => th.Turn)
                .HasForeignKey(th => th.TurnId)
                .OnDelete(DeleteBehavior.Cascade);

            // LegStats configuration
            modelBuilder.Entity<LegStats>()
                .HasKey(ls => ls.Id);

            // SetStats configuration
            modelBuilder.Entity<SetStats>()
                .HasKey(ss => ss.Id);

            // GameStats configuration
            modelBuilder.Entity<GameStats>()
                .HasKey(gs => gs.Id);
           
            modelBuilder.Entity<LegStats>()
                .Property(ls => ls.PPD)
                .HasPrecision(8, 2);
            modelBuilder.Entity<LegStats>()
                .Property(ls => ls.First9PPD)
                .HasPrecision(8, 2);

           

          
            modelBuilder.Entity<SetStats>()
                .Property(ss => ss.PPD)
                .HasPrecision(8, 2);
            modelBuilder.Entity<SetStats>()
                .Property(ss => ss.First9PPD)
                .HasPrecision(8, 2);

         
            modelBuilder.Entity<GameStats>()
                .Property(gs => gs.PPD)
                .HasPrecision(8, 2);
            modelBuilder.Entity<GameStats>()
                .Property(gs => gs.First9PPD)
                .HasPrecision(8, 2);
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

            modelBuilder.Entity<Turn>()
                .HasIndex(t => t.LegId);
          
            modelBuilder.Entity<Leg>()
                .HasIndex(l => l.SetId);

            modelBuilder.Entity<Set>()
                .HasIndex(s => s.GameId);

            modelBuilder.Entity<LegStats>()
                .HasIndex(ls => new { ls.GamePlayerId, ls.LegId });
        
            modelBuilder.Entity<SetStats>()
                .HasIndex(ss => new { ss.GamePlayerId, ss.SetId });
          
            modelBuilder.Entity<GameStats>()
                .HasIndex(gs => new { gs.GamePlayerId, gs.GameId });

            modelBuilder.Entity<GamePlayer>()
                .HasIndex(gp => gp.PlayerId);

          

            modelBuilder.Entity<LegStats>()
                .HasIndex(ls => ls.LegId);  

            modelBuilder.Entity<SetStats>()
                .HasIndex(ss => ss.SetId);  

            modelBuilder.Entity<Turn>()
                .HasIndex(t => t.PlayerId);  


        }

    }
}
