using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicMngr.DTO;
using MusicMngr.Helpers;
using MusicMngr.Models;
using MusicMngr.Request;
using MusicMngr.Responses;
using MusicMngr.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicMngr.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/")]
    public class MusicUserController : Controller
    {
        private IMusicUserService _userService;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;

        public MusicUserController(IMusicUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var authResponse = await _userService.Login(username, password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MusicUserDTO userDto, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            // map dto to entity
            var user = _mapper.Map<MusicUser>(userDto);

            var authResponse = await _userService.Register(user, password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Created(String.Format("/Users/{0}", user.Id), new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _userService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        //[Authorize(Roles = Role.Admin)]
        [HttpGet]
        [Route("Users")]
        public ActionResult<ICollection<MusicUserDTO>> Get()
        {
            if(!HttpContext.User.IsInRole("Admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            var users = _userService.GetMusicUsers();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet]
        [Route("Users/{id}")]
        public ActionResult<MusicUserDTO> Get(int id)
        {
           /* var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            if (!_userService.isSameUser(id, userId))
            {
                return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
            }*/
            if (id <= 0)
            {
                return NotFound();
            }
            MusicUserDTO user = _userService.GetMusicUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /*  [HttpGet]
          [Route("Users/{id}/Playlists")]
          public ActionResult<IEnumerable<PlaylistDTO>> GetMusicUserPlaylists(int id)
          {
              var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
              if (!_userService.isSameUser(id, userId))
              {
                  return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
              }
              var articles = _userService.GetUserPlaylists(id);
              if (articles == null)
              {
                  return NotFound();
              }
              return Ok(articles);
          }

          [HttpGet]
          [Route("Users/{userId}/Playlists/{playlistId}")]
          public ActionResult<PlaylistDTO> GetMusicUserPlaylist(int userId, int articleId)
          {
              var senderId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
              if (!_userService.isSameUser(userId, senderId))
              {
                  return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
              }
              PlaylistDTO article = _userService.GetUserPlaylists(userId).FirstOrDefault(a => a.Id == articleId);
              if (article == null)
              {
                  return NotFound();
              }
              return Ok(article);
          }*/

        /* [HttpGet]
          [Route("Users/{userId}/Playlists/{playlistId}/Songs")]
          public ActionResult<IEnumerable<SongDTO>> GetMusicUserPlaylistSongs(int userId, int articleId)
          {
              var senderId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
              if (!_userService.isSameUser(userId, senderId))
              {
                  return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
              }
              var comments = _userService.GetUserPlaylistSongs(userId, articleId);
              if (comments == null)
              {
                  return BadRequest();
              }
              return Ok(comments);
          }*/

        /*  [HttpGet]
          [Route("Users/{userId}/Playlists/{playlistId}/Songs/{songId}")]
          public ActionResult<SongDTO> GetMusicUserPlaylistSong(int userId, int articleId, int commentId)
          {
              var senderId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
              if (!_userService.isSameUser(userId, senderId))
              {
                  return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
              }
              var comment = _userService.GetUserPlaylistSongs(userId, articleId).FirstOrDefault(c => c.Id == commentId);
              if (comment == null)
              {
                  return BadRequest();
              }
              return Ok(comment);
          }*/

        [HttpPut]
        [Route("Users/{id}")]
        public async Task<ActionResult<MusicUserDTO>> Put(int id, [FromBody] MusicUserDTO user)
        {
            /*var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            if (!_userService.isSameUser(id, userId))
            {
                return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
            }*/
            if (user == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            MusicUserDTO existingUser = await _userService.PutUser(id, user);
            if (existingUser == null)
            {
                return NotFound();
            }
            return Ok(existingUser);
        }

        [HttpDelete]
        [Route("Users/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            /*var userId = HttpContext.User.Claims.Single(x => x.Type == "id").Value;
            var isAdmin = HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.Role).Value;
            if (isAdmin != "Admin")
            {
                if (!_userService.isSameUser(id, userId))
                {
                    return BadRequest(new ErrorResponse(new ErrorMessage { Message = "You are not this user" }));
                }
            }*/

            if (id <= 0)
            {
                return NotFound();
            }
            MusicUser deletedUser = await _userService.DeleteUser(id);
            if (deletedUser == null)
            {
                return NotFound();
            }
            return Ok(deletedUser);
        }

        ~MusicUserController()
        {
        }
        
    }
}
