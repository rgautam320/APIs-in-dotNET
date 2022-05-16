using System;
using System.Linq;
using System.Threading.Tasks;
using Hotel_Booking.Data;
using Hotel_Booking.Models;
using Hotel_Booking.RequestResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace Hotel_Booking.Controllers
{
     public class PaymentTypesController : ControllerBase
     {
          private readonly HotelBookingDBContext _context;
          public PaymentTypesController(HotelBookingDBContext _context)
          {
               this._context = _context;
          }

          [HttpPost, Authorize, RequiredScope("AdminRole"), Route("api/payment-types/create")]
          public async Task<ActionResult> CreatePaymentTypes([FromBody] PaymentTypesModel PaymentTypes)
          {
               try
               {
                    if (PaymentTypes == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment Type object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.PaymentTypes.Any(e => e.PaymentType.ToLower() == PaymentTypes.PaymentType.ToLower()))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type with the same name already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.PaymentTypes.Add(PaymentTypes);
                    await _context.SaveChangesAsync();

                    var PaymentTypeCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment Type created successfully.",
                         Data = PaymentTypes
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

          [HttpGet, Route("api/payment-types/get")]
          public async Task<IActionResult> GetAllPaymentTypes()
          {
               try
               {
                    var Data = await _context.PaymentTypes.ToListAsync();

                    var AllPaymentTypesResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "All Payment Types fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllPaymentTypesResponse);
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

          [HttpGet, Authorize, RequiredScope("UserRole"), Route("api/payment-types/get/{PaymentTypeId}")]
          public async Task<IActionResult> GetSinglePaymentType(int PaymentTypeId)
          {
               try
               {
                    var Data = await _context.PaymentTypes.FindAsync(PaymentTypeId);

                    if (Data == null)
                    {
                         var NoPaymentTypeResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "No Payment Type with the specified Payment Type Id found.",
                         };
                         return StatusCode(400, NoPaymentTypeResponse);
                    }

                    var AllPaymentTypesResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment Type with given Payment Type ID fetched successfully.",
                         Data = Data
                    };
                    return Ok(AllPaymentTypesResponse);
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

          [HttpPut, Authorize, RequiredScope("AdminRole"), Route("api/payment-types/update/{PaymentTypeId}")]
          public async Task<ActionResult> UpdatePaymentTypes(int PaymentTypeId, [FromBody] PaymentTypesModel PaymentTypes)
          {
               try
               {
                    if (PaymentTypes == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment Type object can't be empty."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (!_context.PaymentTypes.Any(e => e.PaymentTypeId == PaymentTypeId))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    if (_context.PaymentTypes.Any(e => e.PaymentType.ToLower() == PaymentTypes.PaymentType.ToLower()))
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type with the same name already exists."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    PaymentTypes.PaymentTypeId = PaymentTypeId;
                    _context.Entry(PaymentTypes).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var PaymentTypeCreatedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment Type updated successfully.",
                         Data = PaymentTypes
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

          [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/payment-types/delete/{PaymentTypeId}")]
          public async Task<IActionResult> DeletePaymentType(int PaymentTypeId)
          {
               try
               {
                    var PaymentType = await _context.PaymentTypes.FindAsync(PaymentTypeId);

                    if (PaymentType == null)
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type with the provided Id not found."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    var checkAlreadyUse = _context.Bookings.Where(e => e.PaymentTypeId == PaymentTypeId);

                    if (checkAlreadyUse.Any())
                    {
                         var errorResponse = new DigitalFailureResponse
                         {
                              Success = false,
                              Message = "Payment type in use. Can't delete."
                         };
                         return StatusCode(400, errorResponse);
                    }

                    _context.PaymentTypes.Remove(PaymentType);
                    await _context.SaveChangesAsync();

                    var PaymentTypeDeletedResponse = new DigitalSuccessResponse
                    {
                         Success = true,
                         Message = "Payment Type deleted successfully.",
                         Data = null
                    };
                    return Ok(PaymentTypeDeletedResponse);
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