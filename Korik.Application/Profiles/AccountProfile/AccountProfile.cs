using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // RegisterDTO => UserEntity || UserEntity => RegisterDTO
            CreateMap<ApplicationUser , RegisterDTO>().ReverseMap();

        }
    }
}
