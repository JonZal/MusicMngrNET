using System.Collections.Generic;
using System.Threading.Tasks;
using MusicMngr.DTO;
using MusicMngr.Models;

namespace MusicMngr.Services
{
    public interface IPlaylistService
    {
        PlaylistDTO GetPlaylist(int userId, int id);
        ICollection<PlaylistDTO> GetPlaylists(int userId);
        ICollection<SongDTO> GetPlaylistSongs(int userId, int id);
        Task<PlaylistDTO> PostPlaylist(int userId, PlaylistDTO playlist);
        Task<PlaylistDTO> PutPlaylist(int userId, int id, PlaylistDTO playlist);
        Task<Models.Playlist> DeletePlaylist(int userId, int id);
        bool UserOwnsPlaylist(int playlistId, int userId);
    }
}