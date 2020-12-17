using System.Collections.Generic;
using MusicMngr.DTO;
using MusicMngr.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MusicMngr.Services
{
    public interface IMusicUserService
    {
        Task<AuthenticationDTO> Login(string username, string password);
        Task<AuthenticationDTO> Register(MusicUser user, string password);
        Task<AuthenticationDTO> RefreshTokenAsync(string token, string refreshToken);
        MusicUserDTO GetMusicUser(long id);
        ICollection<MusicUserDTO> GetMusicUsers();
        ICollection<PlaylistDTO> GetUserPlaylists(int id);
        ICollection<SongDTO> GetUserPlaylistSongs(int userId, int playlistId);
        Task PostUser(MusicUser user);
        Task<MusicUserDTO> PutUser(int id, MusicUser user);
        Task<MusicUser> DeleteUser(int id);
        bool isSameUser(int id, string userId);
    }
}