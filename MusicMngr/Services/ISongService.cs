using System.Collections.Generic;
using MusicMngr.DTO;
using MusicMngr.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MusicMngr.Services
{
    public interface ISongService
    {
        SongDTO GetSong(int id);
        ICollection<SongDTO> GetSongs();
        Task<SongDTO> PostSong(int userId, int plaulistId, Models.Song song);
        Task<SongDTO> PutSong(int id, Models.Song song);
        Task<Models.Song> DeleteSong(int id);
        bool UserOwnsSong(int songId, int userId);
    }
}