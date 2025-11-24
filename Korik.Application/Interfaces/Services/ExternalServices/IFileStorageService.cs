using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Korik.Application
{
    public interface IFileStorageService
    {
        Task<ServiceResult<string>> SaveFileAsync(IFormFile file, string folder);

        Task<bool> DeleteFileAsync(string filePath);

        string GetFileUrl(string filePath);
    }
}