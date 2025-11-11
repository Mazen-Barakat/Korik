using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }


        // Factory helpers
        public static ServiceResult<T> Ok(T data, string message = "Success")
            => new ServiceResult<T> { Success = true, Data = data, Message = message };

        public static ServiceResult<T> Created(T data, string message = "Created")
            => new ServiceResult<T> { Success = true, Data = data, Message = message };

        public static ServiceResult<T> Accepted(string message = "Accepted")
            => new ServiceResult<T> { Success = true, Message = message };

        public static ServiceResult<T> Fail(string message)
            => new ServiceResult<T> { Success = false, Message = message };
    }
}
