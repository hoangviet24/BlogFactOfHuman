using FactOfHuman.Repository.IService;

namespace FactOfHuman.Repository.Service
{
    public class FileService : IFileSerivce
    {
        private readonly IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(webrootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                try { File.Delete(fullPath); }
                catch (Exception ex) { Console.WriteLine($"Không xóa được ảnh: {ex.Message}"); }
            }
        }

        public string? SaveFile(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0) return null;

            var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadFolder = Path.Combine(webrootPath, folderPath);
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return $"/{folderPath}/{fileName}";
        }
    }
}
