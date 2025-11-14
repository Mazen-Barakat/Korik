using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Korik.Application
{
    public class GetAllCarDTOValidator : AbstractValidator<GetAllCarDTO>
    {
        public GetAllCarDTOValidator()
        {
            // No validation rules needed for GetAllCarDTO.
        }
    }
}
