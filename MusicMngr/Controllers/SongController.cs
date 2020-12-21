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
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/Users/{userId}/Playlists/{playlistId}/")]
    public class SongController : ControllerBase
    {
        private ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }
        
        [HttpGet]
        [Route("Songs")]
        public ActionResult<ICollection<SongDTO>> Get(int userId, int playlistId)
        {
            var songs = _songService.GetSongs(userId, playlistId);
            if(songs.Count <= 0)
            {
                return NotFound();
            }
            return Ok(songs);
        }

        // [Authorize(Roles = Role.User)]
        [HttpGet]
        [Route("Songs/{id}")]
        public ActionResult<SongDTO> Get(int userId, int playlistId, int id)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            /*var userOwnsPost = _songService.UserOwnsSong(userId, playlistId, id);
            if (!userOwnsPost)
            {
                return Unauthorized(); // BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }*/
            if (id <= 0)
            {
                return NotFound();
            }
            SongDTO song = _songService.GetSong(userId, playlistId, id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }

        /// <summary>
        /// Creates a Song.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Song
        ///     {
        ///        "name": "Song Name",
        ///        "author": "Test Author"
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="403">The user doesn't have access</response> 
        [HttpPost]
        [Route("Songs")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SongDTO>> Post(int userId, int playlistId, [FromBody] SongDTO song)
        {
            if (song.Author == null && song.Name == null && song.PlaylistId <= 0 && song.UserId <= 0)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newSong = await _songService.PostSong(userId, playlistId, song);
            return Created(String.Format("/Users/{0}/Playlists/{1}/Songs{2}", userId, playlistId, newSong.Id), newSong);
        }

        [HttpPut]
        [Route("Songs/{id}")]
        public async Task<ActionResult<SongDTO>> Put(int userId, int playlistId, int id, [FromBody] SongDTO song)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            /*var userOwnsPost = _songService.UserOwnsSong(userId, playlistId, id);
            if (!userOwnsPost)
            {
                return Unauthorized(); // BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }*/
            if (song == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newSong = await _songService.PutSong(userId, playlistId, id, song);
            var checkSong = _songService.GetSong(id);
            return Ok(checkSong);
        }

        [HttpDelete]
        [Route("Songs/{id}")]
        public async Task<ActionResult> Delete(int userId, int playlistId, int id)
        {
            //var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            /*var userOwnsPost = _songService.UserOwnsSong(userId, playlistId, id);
            if (!userOwnsPost)
            {
                return Unauthorized();  //BadRequest(new ErrorResponse(new ErrorMessage { Message = "You do not own this song" }));
            }*/
            if (id <= 0)
            {
                return NotFound();
            }
            var song = _songService.GetSong(userId, playlistId, id);
            if(song == null)
            {
                return NotFound();
            }
            var deletedSong = await _songService.DeleteSong(userId, playlistId, id);
            return NoContent();
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
