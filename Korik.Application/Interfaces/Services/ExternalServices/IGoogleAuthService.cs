using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IGoogleAuthService
    {
        Task<ServiceResult<UserDTO>> GoogleLoginAsync(GoogleLoginDTO model);
    }
}
