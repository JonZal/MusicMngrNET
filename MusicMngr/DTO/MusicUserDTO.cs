using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.DTO
{
    public class MusicUserDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public ICollection<PlaylistDTO> Playlists { get; set; }
        public ICollection<SongDTO> Songs { get; set; }

    }
}
