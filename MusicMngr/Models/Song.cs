using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Models
{
    [Table("Songs")]
    public class Song
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public MusicUser User { get; set; }
        public Playlist Playlist { get; set; }
    }
}
