using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMngr.DTO;
using MusicMngr.Models;
using MusicMngr.Responses;
using MusicMngr.Services;

namespace MusicMngr.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/Users/{userId}/")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {

            private IPlaylistService _playlistService;

            public PlaylistsController(IPlaylistService playlistService)
            {
                _playlistService = playlistService;
            }

            //[Authorize(Roles = Role.Admin)]
            [HttpGet]
            [Route("Playlists")]
            public ActionResult<ICollection<PlaylistDTO>> Get(int userId)
            {
                return Ok(_playlistService.GetPlaylists(userId));
            }

            [HttpGet]
            [Route("Playlists/{id}")]
            public ActionResult<PlaylistDTO> Get(int userId, int id)
            {
               // var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
                // var userOwnsPost = _playlistService.UserOwnsPlaylist(id, userId);
                // if (!userOwnsPost)
                // {
                //     return BadRequest(new ErrorResponse(new ErrorModel { Message = "You do not own this playlist" }));
                // }
                if (id <= 0)
                {
                    return NotFound();
                }
                PlaylistDTO playlist = _playlistService.GetPlaylists(userId).FirstOrDefault(a => a.Id == id);
                if (playlist == null)
                {
                    return NotFound();
                }
                return Ok(playlist);
            }

           /* [HttpGet]
            [Route("Playlists/{id}/Songs")]
            public ActionResult<IEnumerable<SongDTO>> GetPlaylistSongs(int userId, int id)
            {
                //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
                // var userOwnsPost = _playlistService.UserOwnsPlaylist(id, userId);
                // if (!userOwnsPost)
                // {
                //     return BadRequest(new ErrorResponse(new ErrorModel { Message = "You do not own this playlist" }));
                // }
                var songs = _playlistService.GetPlaylistSongs(userId, id);
                if (songs == null)
                {
                    return NotFound();
                }
                return Ok(songs);
            }*/

          /*  [HttpGet]
            [Route("Playlists/{playlistId}/Songs/{songId}")]
            public ActionResult<SongDTO> GetPlaylistSong(int userId, int playlistId, int songId)
            {
               // var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
                var userOwnsPost = _playlistService.UserOwnsPlaylist(playlistId, userId);
                if (!userOwnsPost)
                {
                    return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this playlist" }));
                }
                if (playlistId <= 0 || songId <= 0)
                {
                    return BadRequest();
                }
                SongDTO song = _playlistService.GetPlaylistSongs(userId, playlistId)
                .FirstOrDefault(c => c.Id == songId);
                if (song == null)
                {
                    return NotFound();
                }
                return Ok(song);
            }*/

            [HttpPost]
            [Route("Playlists")]
            public async Task<ActionResult> Post(int userId, [FromBody] Models.Playlist playlist)
            {
                if (playlist == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newPlaylist = await _playlistService.PostPlaylist(userId, playlist);
                if (newPlaylist == null)
                {
                    return NotFound();
                }
                return Ok(_playlistService.GetPlaylist(userId, (int)newPlaylist.Id));
            }

            [HttpPut]
            [Route("Playlists/{id}")]
            public async Task<ActionResult> Put(int userId, int id, [FromBody] Models.Playlist playlist)
            {
                //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
                var userOwnsPost = _playlistService.UserOwnsPlaylist(id, userId);
                if (!userOwnsPost)
                {
                    return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this playlist" }));
                }
                if (playlist == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newPlaylist = await _playlistService.PutPlaylist(userId, id, playlist);
                return Ok(newPlaylist);
            }

            [HttpDelete]
            [Route("Playlists/{id}")]
            public async Task<ActionResult> DeletePlaylist(int userId, int id)
            {
                //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
                var userOwnsPost = _playlistService.UserOwnsPlaylist(id, userId);
                if (!userOwnsPost)
                {
                    return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this playlist" }));
                }
                if (id <= 0)
                {
                    return NotFound();
                }
                var deletedPlaylist = await _playlistService.DeletePlaylist(userId, id);
                return Ok(deletedPlaylist);
            }

            ~PlaylistsController()
            {
            }

            /*
            private readonly PlaylistContext _context;

            public PlaylistsController(PlaylistContext context)
            {
                _context = context;
            }

            // GET: api/Playlists
            [HttpGet]
            public async Task<ActionResult<IEnumerable<PlaylistDTO>>> GetPlaylists(long userId)
            {
                return await _context.Playlists.Select(x => PlaylistToDTO(x)).Where(x => x.UserId == userId)
                    .ToListAsync();
            }

            // GET: api/Playlists/5
            [HttpGet("{id}")]
            public async Task<ActionResult<PlaylistDTO>> GetPlaylist(long id, long userId)
            {
                var playlist = await _context.Playlists.FindAsync(id);

                if (playlist == null)
                {
                    return NotFound();
                }

                return PlaylistToDTO(playlist);
            }

            // PUT: api/Playlists/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPut("{id}")]
            public async Task<IActionResult> PutPlaylist(long id, PlaylistDTO playlistDTO)
            {
                if (id != playlistDTO.Id)
                {
                    return BadRequest();
                }

                var playlist = await _context.Playlists.FindAsync(id);
                if(playlist == null)
                {
                    return NotFound();
                }

                playlist.UserId = playlistDTO.UserId;
                playlist.Name = playlistDTO.Name;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            // POST: api/Playlists
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
            [HttpPost]
            public async Task<ActionResult<PlaylistDTO>> PostPlaylist(long userId, PlaylistDTO playlistDTO)
            {
                var playlist = new MusicMngr.Models.Playlist
                {
                    UserId = playlistDTO.UserId,
                    Name = playlistDTO.Name
                };
                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPlaylist", new { id = playlist.Id }, PlaylistToDTO(playlist));
            }

            // DELETE: api/Playlists/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePlaylist(long id)
            {
                var playlist = await _context.Playlists.FindAsync(id);
                if (playlist == null)
                {
                    return NotFound();
                }

                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool PlaylistExists(long id)
            {
                return _context.Playlists.Any(e => e.Id == id);
            }

            private static PlaylistDTO PlaylistToDTO(MusicMngr.Models.Playlist playlist) =>
                new PlaylistDTO
                {
                    Id = playlist.Id,
                    UserId = playlist.User.Id,
                    Title = playlist.Title
                };*/
        }
}
