using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking.Controllers
{
     public class RoomTypesController : ControllerBase
     {
          private readonly HotelBookingDBContext _context;

          public RoomTypesController(HotelBookingDBContext context)
          {
               this._context = context;
          }

          [HttpGet, Route("api/room-types/get")]
          public async Task<IActionResult> GetAllRoomTypes()
          {
               try
               {
                    var Data = await _context.RoomTypes.ToListAsync();

                    var AllRoomTypesResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "All Room Types fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllRoomTypesResponse);
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/room-types/get/{RoomTypeId}")]
          public async Task<IActionResult> GetSingleRoomType(int RoomTypeId)
          {
               try
               {
                    var Data = await _context.RoomTypes.FindAsync(RoomTypeId);

                    if (Data == null)
                    {
                         var NoRoomTypeResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Room Type with the specified Room Type Id found.",
                         };
                         return StatusCode(400, NoRoomTypeResponse);
                    }

                    var AllRoomTypesResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room Type with given Room Type ID fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllRoomTypesResponse);
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

          [HttpPost, Authorize, RequiredScope("AdminRole"), Route("api/room-types/create")]
          public async Task<ActionResult> CreateRoomTypes([FromBody] RoomTypesModel RoomTypes)
          {
               try
               {
                    if (RoomTypes == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room Type object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.RoomTypes.Any(e => e.RoomType.ToLower() == RoomTypes.RoomType.ToLower()))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room type with the same name already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.RoomTypes.Add(RoomTypes);
                    await _context.SaveChangesAsync();

                    var RoomTypeCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room Type created successfully.",
                         Data = RoomTypes
                    };
                    return Ok(RoomTypeCreatedResponse);
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

          [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/room-types/update/{RoomTypeId}")]
          public async Task<ActionResult> UpdateRoomTypes(int RoomTypeId, [FromBody] RoomTypesModel RoomTypes)
          {
               try
               {
                    if (RoomTypes == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room Type object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (!_context.RoomTypes.Any(e => e.RoomTypeId == RoomTypeId))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room type with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.RoomTypes.Any(e => e.RoomType.ToLower() == RoomTypes.RoomType.ToLower()))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room type with the same name already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    RoomTypes.RoomTypeId = RoomTypeId;
                    _context.Entry(RoomTypes).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var RoomTypeCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room Type updated successfully.",
                         Data = RoomTypes
                    };
                    return Ok(RoomTypeCreatedResponse);
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

          [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/room-types/delete/{RoomTypeId}")]
          public async Task<IActionResult> DeleteRoomType(int RoomTypeId)
          {
               try
               {
                    var RoomType = await _context.RoomTypes.FindAsync(RoomTypeId);

                    if (RoomType == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room type with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var checkAlreadyUse = _context.Rooms.Where(e => e.RoomTypeId == RoomTypeId);

                    if (checkAlreadyUse.Any())
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room type in use. Can't delete."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.RoomTypes.Remove(RoomType);
                    await _context.SaveChangesAsync();

                    var RoomTypeDeletedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Room Type deleted successfully.",
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
