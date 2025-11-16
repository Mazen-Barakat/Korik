using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SubcategoryProfile : Profile
    {
        public SubcategoryProfile()
        {
            CreateMap<CreateSubcategoryDTO, Subcategory>().ReverseMap();

            CreateMap<UpdateSubcategoryDTO, Subcategory>().ReverseMap();

            CreateMap<Subcategory, SubcategoryDTO>().ReverseMap();
        }
    }
}
