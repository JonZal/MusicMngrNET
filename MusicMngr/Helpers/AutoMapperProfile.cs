using AutoMapper;
using MusicMngr.DTO;
using MusicMngr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMngr.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MusicUser, MusicUserDTO>();
            CreateMap<MusicUserDTO, MusicUser>();
        }
    }
}
