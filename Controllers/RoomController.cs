using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using Hotel_Booking.Service;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking.Controllers
{
     public class RoomController : ControllerBase
     {
          private readonly HotelBookingDBContext _context;
          private readonly IStorageService _uploadService;
          public RoomController(HotelBookingDBContext _context, IStorageService _uploadService)
          {
               this._context = _context;
               this._uploadService = _uploadService;
          }

          [HttpGet, Route("api/rooms/get")]
          public async Task<IActionResult> GetAllRooms()
          {
               try
               {
                    var Data = await _context.Rooms.ToListAsync();

                    var AllRoomsResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "All Rooms fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllRoomsResponse);
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/rooms/get/{RoomId}")]
          public async Task<IActionResult> GetSingleRoom(int RoomId)
          {
               try
               {
                    var Data = await _context.Rooms.FindAsync(RoomId);

                    if (Data == null)
                    {
                         var NoRoomResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Room with the specified Room Id found.",
                         };
                         return StatusCode(400, NoRoomResponse);
                    }

                    var AllRoomsResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room with given Room ID fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllRoomsResponse);
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


          [HttpPost, Authorize, RequiredScope("AdminRole"), Route("api/rooms/create")]
          public async Task<ActionResult> CreateRoom([FromBody] RoomModel Room)
          {
               try
               {
                    if (Room == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.Rooms.Any(e => e.RoomNumber == Room.RoomNumber))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room with the same room number already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.Rooms.Add(Room);
                    await _context.SaveChangesAsync();

                    var RoomCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room created successfully.",
                         Data = Room
                    };
                    return Ok(RoomCreatedResponse);
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

          [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/rooms/update/{RoomId}")]
          public async Task<ActionResult> UpdateRoom(int RoomId, [FromBody] RoomModel Room)
          {
               try
               {
                    if (Room == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (!_context.Rooms.Any(e => e.RoomId == RoomId))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.Rooms.Where(e => e.RoomNumber == Room.RoomNumber).Count() > 1)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room with the same room number already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    Room.RoomId = RoomId;
                    _context.Entry(Room).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room updated successfully.",
                         Data = Room
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

          [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/rooms/delete/{RoomId}")]
          public async Task<IActionResult> DeleteRoom(int RoomId)
          {
               try
               {
                    var Room = await _context.Rooms.FindAsync(RoomId);

                    if (Room == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    // Deleting Room
                    _context.Rooms.Remove(Room);
                    await _context.SaveChangesAsync();
                    
                    // Deleting Images
                    var RoomImages = await _context.RoomImages.Where(e => e.RoomId == RoomId).ToListAsync();

                    foreach (var RoomImage in RoomImages)
                    {    
                         _context.RoomImages.Remove(RoomImage);
                    }
                    await _context.SaveChangesAsync();

                    var RoomTypeDeletedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room deleted successfully.",
                         Data = null
                    };
                    return Ok(RoomTypeDeletedResponse);
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