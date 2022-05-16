using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using System.Linq;

namespace Hotel_Booking.Controllers
{
     public class BookingController : ControllerBase
     {
          private readonly HotelBookingDBContext _context;
          public BookingController(HotelBookingDBContext _context)
          {
               this._context = _context;
          }

          [HttpPost, Authorize, RequiredScope("UserRole"), Route("api/bookings/create")]
          public async Task<ActionResult> CreateBooking([FromBody] BookingModel Booking)
          {
               try
               {
                    if (Booking == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }
                    var FromToDiff = Booking.BookingTo - Booking.BookingFrom;
                    if (FromToDiff.Days > 30 || FromToDiff.Days < 1)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking can't be created for less than a day and more than 30 days."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var Room = await _context.Rooms.FindAsync(Booking.RoomId);
                    if (Room.IsBooked)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Room is already booked."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    // Add Booking
                    _context.Bookings.Add(Booking);
                    await _context.SaveChangesAsync();

                    // Update Room Status                   
                    Room.IsBooked = true;
                    _context.Entry(Room).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Booking created successfully.",
                         Data = Booking
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

          [HttpGet, Authorize, RequiredScope("AdminRole"), Route("api/bookings/get")]
          public async Task<IActionResult> GetAllBookings()
          {
               try
               {
                    var Data = await _context.Bookings.ToListAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "All Bookings fetched successfully.",
                         Data = Data
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/bookings/get/{BookingId}")]
          public async Task<IActionResult> GetSingleBookings(int BookingId)
          {
               try
               {
                    var Data = await _context.Bookings.FindAsync(BookingId);

                    if (Data == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Booking with the given Booking Id."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Booking fetched successfully.",
                         Data = Data
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/bookings/get-by-user/{UserId}")]
          public async Task<IActionResult> GetBookingsByUser(int UserId)
          {
               try
               {
                    var Data = await _context.Bookings.Where(e => e.UserId == UserId).ToListAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Booking fetched successfully.",
                         Data = Data
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

          [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/bookings/update/{BookingId}")]
          public async Task<ActionResult> UpdateBooking(int BookingId, [FromBody] BookingModel Booking)
          {
               try
               {
                    if (Booking == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (!_context.Bookings.Any(e => e.BookingId == BookingId))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    Booking.BookingId = BookingId;
                    _context.Entry(Booking).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var PaymentTypeCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Booking updated successfully.",
                         Data = Booking
                    };
                    return Ok(PaymentTypeCreatedResponse);
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

          [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/bookings/delete/{BookingId}")]
          public async Task<IActionResult> DeleteBooking(int BookingId)
          {
               try
               {
                    var Booking = await _context.Bookings.FindAsync(BookingId);

                    if (Booking == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var checkAlreadyUse = _context.Payments.Where(e => e.BookingId == BookingId);

                    if (checkAlreadyUse.Any())
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type in use. Can't delete."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.Bookings.Remove(Booking);
                    await _context.SaveChangesAsync();

                    var BookingDeletedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Booking deleted successfully.",
                         Data = null
                    };
                    return Ok(BookingDeletedResponse);
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