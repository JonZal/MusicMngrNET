using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Models
{
    [Table("MusicUsers")]
    public class MusicUser
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Secret { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
