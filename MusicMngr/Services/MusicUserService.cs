using System.Collections.Generic;
using MusicMngr.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using MusicMngr.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using MusicMngr.DTO;

namespace MusicMngr.Services
{
    public class MusicUserService : IMusicUserService
    {
        private MusicManagerContext _context;
        private TokenValidationParameters _tokenValidationParameters;
        private readonly AppSettings _appSettings;

        public MusicUserService(MusicManagerContext context, TokenValidationParameters tokenValidationParameters, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
            _appSettings = appSettings.Value;
        }
        public async Task<AuthenticationDTO> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new AuthenticationDTO
                {
                    Errors = new[] { "Missing username or password" }
                };

            var user = _context.MusicUsers.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return new AuthenticationDTO
                {
                    Errors = new[] { "User does not exist" }
                };

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return new AuthenticationDTO
                {
                    Errors = new[] { "User/password combination is wrong" }
                };

            var verifiedUser = GetMusicUsers().SingleOrDefault(x => x.Username == username);
            // authentication successful
            return await GenerateAuthenticationResultForUserAsync(verifiedUser);
        }
        public async Task<AuthenticationDTO> Register(MusicUser user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                return new AuthenticationDTO
                {
                    Errors = new[] { "Password is required" }
                };

            if (_context.MusicUsers.Any(x => x.Username == user.Username))
                return new AuthenticationDTO
                {
                    Errors = new[] { "Username " + user.Username + " is already taken" }
                };

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = Role.User;

            _context.MusicUsers.Add(user);
            _context.SaveChanges();

            var newUser = GetMusicUsers().SingleOrDefault(x => x.Username == user.Username);

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }
        public async Task<AuthenticationDTO> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationDTO { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationDTO { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationDTO { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationDTO { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationDTO { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationDTO { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationDTO { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();
            long id = Convert.ToInt64(validatedToken.Claims.Single(x => x.Type == "id").Value);
            var user = GetMusicUser(id);
            return await GenerateAuthenticationResultForUserAsync(user);
        }
        public MusicUserDTO GetMusicUser(long id)
        {
            if (id <= 0)
            {
                return null;
            }
            MusicUserDTO user = GetMusicUsers().FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public ICollection<MusicUserDTO> GetMusicUsers()
        {
            var songs = GetSongs();
            var Playlists = GetPlaylists();
            var users = _context.MusicUsers
            .Select(user => new MusicUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Playlists = null,//Playlists.Where(a => a.UserId == user.Id).ToList(),
                Songs = null,//songs.Where(c => c.UserId == user.Id).ToList(),
                Role = user.Role
            }).ToList();
            return users;
        }
        public ICollection<PlaylistDTO> GetUserPlaylists(int id)
        {
            var songs = GetSongs();
            var Playlists = _context.Playlists.Include(a => a.User)
            .Select(playlist => new PlaylistDTO
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description,
                UserId = playlist.User.Id,
                Songs = null//songs.Where(c => c.PlaylistId == playlist.Id).AsQueryable().ToList()
            }).Where(a => a.UserId == id).ToList();
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
        private ICollection<PlaylistDTO> GetPlaylists()
        {
            var songs = GetSongs();
            var Playlists = _context.Playlists//.Include(a => a.User)
            .Select(playlist => new PlaylistDTO
            {
                Id = playlist.Id,
                Title = playlist.Title,
                Description = playlist.Description,
                UserId = playlist.User.Id,
                Songs = null//songs.AsEnumerable().Where(c => c.PlaylistId == playlist.Id).ToList()
            }).ToList();
            return Playlists;
        }
        public ICollection<SongDTO> GetUserSongss(int id)
        {
            var songs = _context.Songs.Include(c => c.Playlist).Include(c => c.User)
            .Select(song => new SongDTO
            {
                Id = song.Id,
                Name = song.Name,
                Author = song.Author,
                PlaylistId = song.Playlist.Id,
                UserId = song.User.Id
            }).Where(c => c.UserId == id).ToList();
            return songs;
        }
        public ICollection<SongDTO> GetUserPlaylistSongs(int userId, int playllistId)
        {
            var songs = GetUserPlaylists(userId)
            .FirstOrDefault(a => a.Id == playllistId)
            .Songs.ToList();
            if (songs == null)
            {
                return null;
            }
            return songs;
        }
        public async Task PostUser(MusicUser user)
        {
            await _context.MusicUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<MusicUserDTO> PutUser(int id, MusicUser user)
        {
            MusicUser existingUser = _context.MusicUsers.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return null;
            }
            if (user.Name != null)
            {
                existingUser.Name = user.Name;
            }
            if (user.Username != null)
            {
                existingUser.Username = user.Username;
            }
            _context.Attach(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return GetMusicUser(id);
        }
        public async Task<MusicUser> DeleteUser(int id)
        {
            MusicUser user = _context.MusicUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return null;
            }
            List<MusicMngr.Models.Song> songs = _context.Songs.Where(c => c.User.Id == user.Id).ToList();
            songs.ForEach(c => _context.Songs.Remove(c));
            _context.MusicUsers.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public bool isSameUser(int id, string userId)
        {
            return id.ToString() == userId;
        }
        // private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken); if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationDTO> GenerateAuthenticationResultForUserAsync(MusicUserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.Add(_appSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id.ToString(),
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationDTO
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        ~MusicUserService()
        {
            _context.Dispose();
        }
    }
}