using BlogApp.Services.Interfaces;

namespace BlogApp.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultBlogUserImageSrc = "/img/ContactPro.png";
        private readonly string _defaultBlogPostImageSrc = "/img/ContactProLight.png";
        private readonly string _defaultCategoryImageSrc = "/img/DefaultContactImage.png";

        //TO DO: Blog Customizations

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (imageType)
                {

                    case 1: return _defaultBlogUserImageSrc; //BlogUser Image
                    case 2: return _defaultBlogPostImageSrc; //BlogPost Image
                    case 3: return _defaultCategoryImageSrc; //Catagory Image

                }


            }


            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
