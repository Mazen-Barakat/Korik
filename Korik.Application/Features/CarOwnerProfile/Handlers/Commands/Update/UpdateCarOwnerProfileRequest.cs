using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record UpdateCarOwnerProfileRequest(UpdateCarOwnerProfileDTO Model) : IRequest<ServiceResult<CarOwnerProfileDTO>> { }

    public class UpdateCarOwnerProfileRequestHandler
    {
    }
}