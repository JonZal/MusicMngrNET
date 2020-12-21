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
        SongDTO GetSong(int userId, int playlistId, int id);
        ICollection<SongDTO> GetSongs(int userId, int playlistId);
        Task<SongDTO> PostSong(int userId, int plaulistId, SongDTO song);
        Task<SongDTO> PutSong(int userId, int playlistId, int id, SongDTO song);
        Task<Models.Song> DeleteSong(int userId, int playlistId, int id);
        bool UserOwnsSong(int userId, int playlistId, int songId);
    }
}