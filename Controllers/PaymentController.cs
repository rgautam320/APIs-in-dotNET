using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Hotel_Booking.Controllers
{
     public class PaymentController : ControllerBase
     {
          private readonly HotelBookingDBContext _context;
          public PaymentController(HotelBookingDBContext _context)
          {
               this._context = _context;
          }

          [HttpPost, Authorize, RequiredScope("UserRole"), Route("api/payments/create")]
          public async Task<ActionResult> CreatePayment([FromBody] PaymentModel Payment)
          {
               try
               {
                    if (Payment == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var Booking = await _context.Bookings.FindAsync(Payment.BookingId);
                    if (Booking == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Booking exists with the given booking id."
                         };
                         return StatusCode(400, errorResponse);
                    }
                    if (Booking.TotalPrice != Payment.PaymentAmount)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Booking amount and Payment amount didn't match."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (Booking.IsPaid)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "You have already paid for this order."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    Booking.IsPaid = true;
                    Booking.IsConfirmed = true;
                    _context.Entry(Booking).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    _context.Payments.Add(Payment);
                    await _context.SaveChangesAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment made successfully.",
                         Data = Payment
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

          [HttpGet, Authorize, RequiredScope("AdminRole"), Route("api/payments/get")]
          public async Task<IActionResult> GetAllPayments()
          {
               try
               {
                    var Data = await _context.Payments.ToListAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "All Payments fetched successfully.",
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/payments/get/{PaymentId}")]
          public async Task<IActionResult> GetSinglePayment(int PaymentId)
          {
               try
               {
                    var Data = await _context.Payments.FindAsync(PaymentId);

                    if (Data == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Payment with the given Payment Id."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment fetched successfully.",
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/payments/get-by-user/{UserId}")]
          public async Task<IActionResult> GetPaymentsByUser(int UserId)
          {
               try
               {
                    var Data = await _context.Payments.Where(e => e.UserId == UserId).ToListAsync();

                    var successResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment fetched successfully.",
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

          [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/payments/update/{PaymentId}")]
          public async Task<ActionResult> UpdatePayment(int PaymentId, [FromBody] PaymentModel Payment)
          {
               try
               {
                    if (Payment == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (!_context.Payments.Any(e => e.PaymentId == PaymentId))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    Payment.PaymentId = PaymentId;
                    _context.Entry(Payment).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var PaymentSuccessResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment updated successfully.",
                         Data = Payment
                    };
                    return Ok(PaymentSuccessResponse);
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