using System;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Hotel_Booking.Data;
using Hotel_Booking.RequestResponseModel;
using System.IO;

namespace Hotel_Booking.Service
{
     public class StorageService : IStorageService
     {
          private readonly BlobServiceClient _blobServiceClient;
          private readonly IConfiguration _iConfiguration;
          private readonly HotelBookingDBContext _context;

          public StorageService(BlobServiceClient _blobServiceClient, IConfiguration _iConfiguration, HotelBookingDBContext _context)
          {
               this._blobServiceClient = _blobServiceClient;
               this._iConfiguration = _iConfiguration;
               this._context = _context;
          }

          public FileUploadResponse UploadFile(IFormFile formFile)
          {
               try
               {
                    var containerName = _iConfiguration.GetSection("Storage:ContainerName").Value;
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    var blobClient = containerClient.GetBlobClient($"{Guid.NewGuid()}-{formFile.FileName}");

                    using var stream = formFile.OpenReadStream();
                    blobClient.Upload(stream, true);

                    var responseFileUpload = new FileUploadResponse
                    {
                         Success = true,
                         ImageURL = blobClient.Name
                    };

                    return responseFileUpload;
               }
               catch (Exception ex)
               {
                    var responseFileUpload = new FileUploadResponse
                    {
                         Success = false,
                         ImageURL = null
                    };
                    Console.WriteLine(ex.Message);
                    return responseFileUpload;
               }
          }

          public bool DeleteFile(string fileName)
          {
               try
               {
                    var containerName = _iConfiguration.GetSection("Storage:ContainerName").Value;
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    blobClient.DeleteIfExists();

                    return true;
               }
               catch (Exception ex)
               {
                    Console.WriteLine(ex.Message);
                    return false;
               }
          }

          public FileDownloadResponse DownloadFile(string fileName, string ImageType)
          {
               try
               {
                    var containerName = _iConfiguration.GetSection("Storage:ContainerName").Value;
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    var DownloadedFile = blobClient.DownloadContent().Value;

                    var DownloadedFileResponse = new FileDownloadResponse
                    {
                         FileContent = DownloadedFile.Content.ToArray(),
                         Details = DownloadedFile.Details,
                         ImageType = ImageType,
                         FileName = fileName
                    };
                    return DownloadedFileResponse;
               }
               catch (System.Exception ex)
               {
                    Console.WriteLine(ex.Message);
                    return null;
               }
          }
     }
}