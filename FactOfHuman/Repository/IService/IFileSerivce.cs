using Microsoft.AspNetCore.Http;

namespace FactOfHuman.Repository.IService
{
    public interface IFileSerivce
    {
        string? SaveFile(IFormFile file, string folderPath);
        void DeleteFile(string filePath); 
    }
}
