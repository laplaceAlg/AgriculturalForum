namespace AgriculturalForum.Web.Helper
{
    public class ApplicationContext
    {
        private static IWebHostEnvironment? _hostEnvironment;

        public static void Configure(IWebHostEnvironment hostEnvironment)
        {

            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException();
        }
        public static IWebHostEnvironment? HostEnviroment => _hostEnvironment;
        public static string WebRootPath => _hostEnvironment?.WebRootPath ?? string.Empty;

        public static string UploadFile(IFormFile file, string folder, string fileName = "")
        {
            if (fileName == "") fileName = file.FileName;
            if (file != null && file.Length > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".gif"))
                {
                    string filePath = Path.Combine(HostEnviroment.WebRootPath, folder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                else
                    return null;

            }
            return fileName;
        }
    }
}
