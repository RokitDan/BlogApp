namespace BlogApp.Services.Interfaces
{
    public interface IImageService
    {
        //converts uploaded image to byte array
        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);

        //converts byte array from database to image
        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType);
    }
}
