using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Korik.Application
{
    public class GetAllCarExpenseDTOValidator : AbstractValidator<GetAllCarExpenseDTO>
    {
        public GetAllCarExpenseDTOValidator()
        {
            // No validation rules needed for GetAllCarExpenseDTO.
        }
    }
}
