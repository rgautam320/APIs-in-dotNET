using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using Hotel_Booking.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Hotel_Booking.Controllers
{
    public class RoomImagesController : ControllerBase
    {
        private HotelBookingDBContext _context;
        private IStorageService _storageService;

        public RoomImagesController(HotelBookingDBContext _context, IStorageService _storageService)
        {
            this._context = _context;
            this._storageService = _storageService;
        }

        [HttpPost, Authorize, RequiredScope("AdminRole"), Route("api/room-images/create/{RoomId}")]
        public async Task<IActionResult> CreateRoomImage(IFormFile file, int RoomId, [FromQuery(Name = "ImageType")] string ImageType)
        {
            try
            {
                if (file == null)
                {
                    var errorResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "File is required."
                    };
                    return StatusCode(400, errorResponse);
                }

                if (ImageType == "Main")
                {
                    var RoomMainImages = await _context.RoomImages.Where(e => e.RoomId == RoomId).ToListAsync();

                    foreach (var RoomMainImage in RoomMainImages)
                    {
                        var DeleteFile = _storageService.DeleteFile(RoomMainImage.RoomImage);

                        if (DeleteFile)
                        {
                            var checkFile = _context.RoomImages.Where(e => e.RoomImage == RoomMainImage.RoomImage).FirstOrDefault();
                            _context.RoomImages.Remove(checkFile);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                var RoomFile = _storageService.UploadFile(file);

                if (RoomFile.Success)
                {
                    var RoomImage = new RoomImagesModel
                    {
                        RoomImage = RoomFile.ImageURL,
                        ImageType = ImageType,
                        RoomId = RoomId
                    };
                    _context.RoomImages.Add(RoomImage);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var errorResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "File not uploaded"
                    };
                    return StatusCode(400, errorResponse);
                }

                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "File uploaded successfully.",
                    Data = RoomFile.ImageURL
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, errorResponse);
            }
        }

        [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/room-images/delete/{FileName}")]
        public async Task<ActionResult> DeleteRoomImage(string FileName)
        {
            try
            {
                var DeleteFile = _storageService.DeleteFile(FileName);

                if (DeleteFile)
                {
                    var checkFile = _context.RoomImages.Where(e => e.RoomImage == FileName).FirstOrDefault();

                    _context.RoomImages.Remove(checkFile);
                    await _context.SaveChangesAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                        Success = true,
                        Message = "File deleted successfully.",
                        Data = null
                    };
                    return Ok(successResponse);
                }
                else
                {
                    var errorResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "File not deleted"
                    };
                    return StatusCode(400, errorResponse);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, errorResponse);
            }
        }

        [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/room-images/get/{RoomId}")]
        public async Task<ActionResult> GetRoomImages(int RoomId)
        {
            try
            {
                var Images = await _context.RoomImages.Where(e => e.RoomId == RoomId).ToListAsync();

                List<FileDownloadResponse> Files = new();
                foreach (var Image in Images)
                {
                    var file = _storageService.DownloadFile(Image.RoomImage, Image.ImageType);
                    Files.Add(file);
                }
                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "Files downloaded successfully.",
                    Data = Files
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, errorResponse);
            }
        }

        [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/room-images/get-single-image/{ImageName}")]
        public ActionResult GetSingleImage(string ImageName)
        {
            try
            {
                var file = _storageService.DownloadFile(ImageName, "Single Image");

                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "File downloaded successfully.",
                    Data = file
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, errorResponse);
            }
        }

        [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/room-images/update/{Filename}")]
        public async Task<ActionResult> UpdateRoomImage(IFormFile file, string Filename, [FromQuery(Name = "ImageType")] string ImageType)
        {
            try
            {
                var checkFile = _context.RoomImages.Where(e => e.RoomImage == Filename).FirstOrDefault();

                if (checkFile == null)
                {
                    var errorResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "File not found"
                    };
                    return StatusCode(400, errorResponse);
                }

                var RoomFile = _storageService.UploadFile(file);
                if (RoomFile.Success)
                {
                    var RoomImage = new RoomImagesModel
                    {
                        RoomImage = RoomFile.ImageURL,
                        ImageType = ImageType,
                        RoomId = checkFile.RoomId
                    };
                    _context.RoomImages.Add(RoomImage);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var errorUpdateResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "File not updated"
                    };
                    return StatusCode(400, errorUpdateResponse);
                }

                // Delete Previous File
                _storageService.DeleteFile(Filename);

                _context.RoomImages.Remove(checkFile);
                await _context.SaveChangesAsync();

                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "File updated successfully.",
                    Data = RoomFile.ImageURL
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, errorResponse);
            }
        }
    }
}