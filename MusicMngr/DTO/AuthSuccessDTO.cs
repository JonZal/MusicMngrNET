using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.DTO
{
    public class AuthSuccessDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
