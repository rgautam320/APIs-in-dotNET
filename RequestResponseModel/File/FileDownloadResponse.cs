using System;
using System.IO;

namespace Hotel_Booking.RequestResponseModel
{
     public class FileDownloadResponse
     {
          public byte[] FileContent { get; set; }
          public string ImageType { get; set; }
          public object Details { get; set; }
          public string FileName { get; set; }
     }
}