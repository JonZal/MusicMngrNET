using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Models
{
    public class PlaylistContext : DbContext
    {
        public PlaylistContext(DbContextOptions<PlaylistContext> options)
            : base(options)
        {
        }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<DTO.PlaylistDTO> PlaylistDTO { get; set; }

    }
}
