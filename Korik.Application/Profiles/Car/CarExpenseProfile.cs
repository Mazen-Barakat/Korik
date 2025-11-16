using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Korik.Domain;

namespace Korik.Application
{
    public class CarExpenseProfile : Profile
    {
        public CarExpenseProfile()
        {
            // Map CreateCarExpanseDTO -> CarExpenses
            CreateMap<CreateCarExpanseDTO, CarExpenses>();

            // Map CarExpenses -> CarExpenseDTO
            CreateMap<CarExpenses, CarExpenseDTO>();

            // Map UpdateCarExpenseDTO -> CarExpenses
            CreateMap<UpdateCarExpenseDTO, CarExpenses>();
        }
    }
}
