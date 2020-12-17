using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMngr.DTO;
using MusicMngr.Models;

namespace MusicMngr.Controllers
{
    /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/Users")]
    [ApiController]
    public class SkubiMusizka : ControllerBase
    {
        private readonly MusicUserContext _context;

        public SkubiMusizka(MusicUserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MusicUserDTO>>> GetUsers()
        {
            return await _context.MusicUsers
                .Select(x => UserToDTO(x))
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MusicUserDTO>> GetMusicUser(long id)
        {
            var musicUser = await _context.MusicUsers.FindAsync(id);

            if (musicUser == null)
            {
                return NotFound();
            }

            return UserToDTO(musicUser);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMusicUser(long id, MusicUserDTO musicMusicUserDTO)
        {
            if (id != musicMusicUserDTO.Id)
            {
                return BadRequest();
            }

            var user = await _context.MusicUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = musicMusicUserDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MusicUserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MusicUserDTO>> PostMusicUser(MusicUserDTO musicUser)
        {
            var user = new MusicUser
            {
                Name = musicUser.Name
            };

            _context.MusicUsers.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUsers),
                new { id = user.Id },
                UserToDTO(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusicUser(long id)
        {
            var musicUser = await _context.MusicUsers.FindAsync(id);
            if (musicUser == null)
            {
                return NotFound();
            }

            _context.MusicUsers.Remove(musicUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MusicUserExists(long id)
        {
            return _context.MusicUsers.Any(e => e.Id == id);
        }

        private static MusicUserDTO UserToDTO(MusicUser user) =>
            new MusicUserDTO
            {
                Id = user.Id,
                Name = user.Name
            };
    }*/
}
