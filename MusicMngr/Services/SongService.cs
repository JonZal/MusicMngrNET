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
            return GetSongs().FirstOrDefault(c => c.Id == id);
        }

        public ICollection<SongDTO> GetSongs()
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

        public async Task<SongDTO> PostSong(int userId, int plaulistId, MusicMngr.Models.Song song)
        {
            MusicUser user = _context.MusicUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }
            song.User = user;
            MusicMngr.Models.Playlist playlist = _context.Playlists.FirstOrDefault(a => a.Id == plaulistId);
            if (playlist == null)
            {
                return null;
            }
            song.Playlist = playlist;
            await _context.Songs.AddAsync(song);
            await _context.SaveChangesAsync();
            return GetSong((int)song.Id);
        }

        public async Task<SongDTO> PutSong(int id, MusicMngr.Models.Song song)
        {
            MusicMngr.Models.Song existingSong = _context.Songs.FirstOrDefault(c => c.Id == id);
            if (existingSong == null)
            {
                return null;
            }
            if (song.Name != null)
            {
                existingSong.Name = song.Name; 
            }            
            _context.Attach(existingSong).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return GetSong(id);
        }
        public async Task<MusicMngr.Models.Song> DeleteSong(int id)
        {
            MusicMngr.Models.Song song = _context.Songs.FirstOrDefault(c => c.Id == id);
            if (song == null)
            {
                return null;
            }
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return song;
        }
        public bool UserOwnsSong(int songId, int userId)
        {
            var song = GetSong(songId);

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