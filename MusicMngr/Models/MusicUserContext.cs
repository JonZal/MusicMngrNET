using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicMngr.DTO;

namespace MusicMngr.Models
{
    public class MusicUserContext : DbContext
    {
        public MusicUserContext(DbContextOptions<MusicUserContext> options)
            : base(options)
        {
        }

        public DbSet<MusicUser> MusicUsers { get; set; }

        public DbSet<MusicMngr.DTO.MusicUserDTO> MusicUserDTO { get; set; }
    }
}
