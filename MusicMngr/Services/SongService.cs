using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using MusicMngr.Models;
using MusicMngr.DTO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicMngr.Services;

namespace MusicMngr.Services
{
    public class SongService : ISongService
    {
        private MusicManagerContext _context;

        public SongService(MusicManagerContext context)
        {
            _context = context;
        }

        public SongDTO GetSong(int id)
        {
            var gotSong = _context.Songs.Where(a => (a.Id == id))
                .Select(song => new SongDTO
                {
                    Id = song.Id,
                    Name = song.Name,
                    Author = song.Author,
                    PlaylistId = song.Playlist.Id,
                    UserId = song.User.Id
                }).FirstOrDefault();
            return gotSong;
        }
        public SongDTO GetSong(int userId, int playlistId, int id)
        {
            return GetSongs(userId, playlistId).FirstOrDefault(c => c.Id == id);
        }

        public ICollection<SongDTO> GetSongs(int userId, int playlistId)
        {
            var songs = _context.Songs.Where(a => (a.User.Id == userId && a.Playlist.Id == playlistId)).Include(c => c.Playlist).Include(c => c.User)
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

        public async Task<SongDTO> PostSong(int userId, int plaulistId, SongDTO song)
        {
            MusicUser user = _context.MusicUsers.FirstOrDefault(u => u.Id == userId);
            Models.Song newSong = new Models.Song();
            if (user == null)
            {
                return null;
            }
            newSong.User = user;
            Models.Playlist playlist = _context.Playlists.FirstOrDefault(a => a.Id == plaulistId);
            if (playlist == null)
            {
                return null;
            }
            newSong.Playlist = playlist;
            newSong.Name = song.Name;
            newSong.Author = song.Author;
            await _context.Songs.AddAsync(newSong);
            await _context.SaveChangesAsync();
            return GetSong(userId, plaulistId, (int)newSong.Id);
        }

        public async Task<SongDTO> PutSong(int userId, int playlistId, int id, SongDTO song)
        {
            Models.Song existingSong = _context.Songs.Where(a => (a.Playlist.Id == playlistId && a.User.Id == userId)).FirstOrDefault(c => c.Id == id);
            if (existingSong == null)
            {
                return null;
            }
            if (song.Name != null)
            {
                existingSong.Name = song.Name; 
            }
            if (song.Author != null)
            {
                existingSong.Author = song.Author;
            }
            if (song.PlaylistId > 0)
            {
                existingSong.Playlist = _context.Playlists.Where(a => a.Id == song.PlaylistId).FirstOrDefault();
            }
            if (song.UserId > 0)
            {
                existingSong.User = _context.MusicUsers.Where(a => a.Id == song.UserId).FirstOrDefault();
            }
            _context.Attach(existingSong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return GetSong((int)song.UserId, (int)song.PlaylistId, id);
        }
        public async Task<Models.Song> DeleteSong(int userId, int playlistId, int id)
        {
            Models.Song song = _context.Songs.Where(a => (a.User.Id == userId && a.Playlist.Id == playlistId)).FirstOrDefault(c => c.Id == id);
            if (song == null)
            {
                return null;
            }
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return song;
        }
        public bool UserOwnsSong(int userId, int playlistId, int songId)
        {
            var song = GetSong(userId, playlistId, songId);

            if (song == null)
            {
                return false;
            }

            if (song.UserId != userId)
            {
                return false;
            }

            return true;
        }

        ~SongService()
        {
            _context.Dispose();
        }
    }
}