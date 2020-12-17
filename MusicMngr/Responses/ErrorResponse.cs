using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse() { }

        public ErrorResponse(ErrorMessage error)
        {
            Errors.Add(error);
        }

        public List<ErrorMessage> Errors { get; set; } = new List<ErrorMessage>();
    }
}
