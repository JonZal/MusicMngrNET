using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.DTO
{
    public class SongDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public long UserId { get; set; }
        public long PlaylistId { get; set; }
    }
}
