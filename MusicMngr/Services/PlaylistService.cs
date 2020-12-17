using System.Collections.Generic;
using MusicMngr.DTO;
using MusicMngr.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MusicMngr.Services
{
    public class PlaylistService : IPlaylistService
    {
        private MusicManagerContext _context;

        public PlaylistService(MusicManagerContext context)
        {
            _context = context;
        }

        public PlaylistDTO GetPlaylist(int userId, int id)
        {
            if (id <= 0)
            {
                return null;
            }
            PlaylistDTO playlist = GetPlaylists(userId).FirstOrDefault(u => u.Id == id);
            if (playlist == null)
            {
                return null;
            }
            return playlist;
        }
        public ICollection<PlaylistDTO> GetPlaylists(int userId)
        {
            var songs = GetSongs();
            var Playlists = _context.Playlists.Where(a => a.User.Id == userId)
            .Select(playlist => new PlaylistDTO
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description,
                UserId = playlist.User.Id,
                Songs = null//songs.Where(c => c.PlaylistId == playlist.Id).AsQueryable().ToList()
            }).ToList();
            return Playlists;
        }
        private ICollection<SongDTO> GetSongs()
        {
            var songs = _context.Songs.Include(c => c.Playlist).Include(c => c.User)
            .Select(song => new SongDTO
            {
                Id = song.Id,
                Name = song.Name,
                Author = song.Author,
                PlaylistId = song.Playlist.Id,
                UserId = song.User.Id
            }).ToList();
            return songs;
        }
        public ICollection<SongDTO> GetPlaylistSongs(int userId, int id)
        {
            return GetSongs().Where(c => c.PlaylistId == id).ToList();
        }

        public async Task<PlaylistDTO> PostPlaylist(int userId, Models.Playlist playlist)
        {
            MusicUser user = _context.MusicUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }
            playlist.User = user;
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
            return GetPlaylist(userId, (int)playlist.Id);
        }

        public async Task<PlaylistDTO> PutPlaylist(int userId, int id, Models.Playlist playlist)
        {
            Models.Playlist existingPlaylist = _context.Playlists.FirstOrDefault(a => a.Id == id);
            if (existingPlaylist == null)
            {
                return null;
            }
            if (playlist.Title != null)
            {
                existingPlaylist.Title = playlist.Title;
            }
            if (playlist.Description != null)
            {
                existingPlaylist.Description = playlist.Description;
            }
            _context.Attach(existingPlaylist).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return GetPlaylist(userId, id);
        }

        public async Task<Models.Playlist> DeletePlaylist(int userId, int id)
        {
            Models.Playlist playlist = _context.Playlists.FirstOrDefault(a => a.Id == id);
            if (playlist == null)
            {
                return null;
            }
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
            return playlist;
        }

        public bool UserOwnsPlaylist(int playlistId, int userId)
        { 
            var playlist = GetPlaylist(userId, playlistId);

            if (playlist == null)
            {
                return false;
            }

            if (playlist.UserId != userId)
            {
                return false;
            }

            return true;
        }

        ~PlaylistService()
        {
            _context.Dispose();
        }
    }
}