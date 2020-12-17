using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Models
{
    public class MusicManagerContext : DbContext
    {
        public MusicManagerContext(DbContextOptions options) : base(options) { }
        public DbSet<MusicUser> MusicUsers { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Playlist>().ToTable("Playlists")
                .HasMany(a => a.Songs)
                .WithOne(c => c.Playlist)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Song>().ToTable("Songs");
            modelBuilder.Entity<MusicUser>().ToTable("MusicUsers")
                .HasMany(u => u.Playlists)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
        }
    }
}
