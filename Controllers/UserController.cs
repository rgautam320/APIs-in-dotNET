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
    public class UserController : ControllerBase
    {
        private readonly HotelBookingDBContext _context;

        public UserController(HotelBookingDBContext context)
        {
            this._context = context;
        }

        [HttpGet, Authorize, RequiredScope("AdminRole"), Route("api/user/get")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var Users = await _context.Users.ToListAsync();

                var AllUsersResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "All Users fetched successfully.",
                    Data = Users
                };
                return Ok(AllUsersResponse);
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

        [HttpGet, Authorize, RequiredScope("AdminRole"), Route("api/user/get/{Email}")]
        public async Task<IActionResult> GetSingleUser(string Email)
        {
            try
            {
                var User = await _context.Users.FirstOrDefaultAsync(e => e.Email.ToLower() == Email.ToLower());

                if(User == null)
                {
                    var NoUserResponse = new DigitalFailureResponse
                    {
                        Success = false,
                        Message = "No User with the specified Email Id found.",
                    };
                    return StatusCode(400, NoUserResponse);
                }

                var AllUsersResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "User fetched successfully.",
                    Data = User
                };
                return Ok(AllUsersResponse);
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

        [HttpPut, Authorize, RequiredScope("UserRole"), Route("api/user/update/{UserId}")]
        public async Task<ActionResult> UpdateUser(int UserId, [FromBody] UserModel user)
        {
            if (user == null)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = "User object can't be empty."
                };
                return StatusCode(400, errorResponse);
            }

            if (!_context.Users.Any(e => e.UserId == UserId))
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = "User not found."
                };
                return StatusCode(400, errorResponse);
            }
            user.UserId = UserId;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "User updated successfully.",
                    Data = null
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var updateErrorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, updateErrorResponse);
            }
        }

        [HttpDelete, Authorize, RequiredScope("AdminRole"), Route("api/user/delete/{UserId}")]
        public async Task<IActionResult> DeleteCourse(int UserId)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = "User not found."
                };
                return StatusCode(400, errorResponse);
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                var successResponse = new DigitalSuccessResponse
                {
                    Success = true,
                    Message = "User deleted successfully.",
                    Data = null
                };
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var deleteErrorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(400, deleteErrorResponse);
            }
        }
    }
}
