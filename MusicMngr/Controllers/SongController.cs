using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicMngr.DTO;
using MusicMngr.Models;
using MusicMngr.Responses;
using MusicMngr.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/Users/{userId}/Playlists/{playlistId}/")]
    public class SongController : ControllerBase
    {
        private ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        [Route("Songs")]
        public ActionResult<ICollection<SongDTO>> Get()
        {
            return Ok(_songService.GetSongs());
        }

        [HttpGet]
        [Route("Songs/{id}")]
        public ActionResult<SongDTO> Get(int userId, int playlistId, int id)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            var userOwnsPost = _songService.UserOwnsSong(id, userId);
            if (!userOwnsPost)
            {
                return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }
            if (id <= 0)
            {
                return NotFound();
            }
            SongDTO song = _songService.GetSong(id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }

        [HttpPost]
        [Route("Songs")]
        public async Task<ActionResult<SongDTO>> Post(int userId, int playlistId, [FromBody] Models.Song song)
        {
            if (song == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSong = await _songService.PostSong(userId, playlistId, song);
            return Ok(newSong);
        }

        [HttpPut]
        [Route("Songs/{id}")]
        public async Task<ActionResult<SongDTO>> Put(int userId, int playlistId, int id, [FromBody] Models.Song song)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            var userOwnsPost = _songService.UserOwnsSong(id, userId);
            if (!userOwnsPost)
            {
                return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }
            if (song == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newSong = await _songService.PutSong(id, song);
            return Ok(newSong);
        }

        [HttpDelete]
        [Route("Songs/{id}")]
        public async Task<ActionResult> Delete(int userId, int playlistId, int id)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            var userOwnsPost = _songService.UserOwnsSong(id, userId);
            if (!userOwnsPost)
            {
                return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }
            if (id <= 0)
            {
                return NotFound();
            }
            var deletedSong = await _songService.DeleteSong(id);
            return Ok(deletedSong);
        }

        private ActionResult BadRequest(ErrorResponse errorResponse)
        {
            throw new NotImplementedException();
        }

        ~SongController()
        {
        }
    }
}
