using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.DTO
{
    public class PlaylistDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long UserId { get; set; }
        public ICollection<long> Songs { get; set; }
    }
}
