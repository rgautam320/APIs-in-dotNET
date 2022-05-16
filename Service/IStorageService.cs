using Hotel_Booking.RequestResponseModel;
using Microsoft.AspNetCore.Http;

namespace Hotel_Booking.Service
{
     public interface IStorageService
     {
          FileUploadResponse UploadFile(IFormFile file);
          bool DeleteFile(string fileName);
          FileDownloadResponse DownloadFile(string FileName, string ImageType);
     }
}