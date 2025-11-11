using Korik.Application;
using Microsoft.AspNetCore.Mvc;

namespace Korik.API
{ 
    public static class ApiResponse
    {
        public static IActionResult FromResult<T>(ControllerBase controller, ServiceResult<T> result)
        {
            if (result == null)
            {
                return controller.StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = "Unexpected null result" });
            }

            if (result.Success)
            {
                var statusCode = result.Message switch
                {
                    "Created" => StatusCodes.Status201Created,
                    "Accepted" => StatusCodes.Status202Accepted,
                    _ => StatusCodes.Status200OK
                };

                return controller.StatusCode(statusCode, new
                {
                    result.Success,
                    Message = string.IsNullOrWhiteSpace(result.Message) ? "Success" : result.Message,
                    result.Data
                });
            }

            return controller.BadRequest(new
            {
                result.Success,
                Message = result.Message ?? "Request failed",
                result.Data
            });
        }
    }
}
