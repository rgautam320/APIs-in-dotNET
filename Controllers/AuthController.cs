using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using Hotel_Booking.Constants;
using Hotel_Booking.RequestResponseModel;
using Hotel_Booking.RequestResponseModel.Auth;
using Hotel_Booking.Data;
using Hotel_Booking.Models;
using System.Linq;

namespace Hotel_Booking.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IOptions<GraphAPISettings> graphAPISettings;
        private readonly HotelBookingDBContext _context;

        public AuthController(IOptions<GraphAPISettings> graphAPISettings, HotelBookingDBContext context)
        {
            this.graphAPISettings = graphAPISettings;
            this._context = context;
        }

        [HttpPost]
        [Route("api/login/user/code")]
        public async Task<ActionResult> UserLoginTokenMethod([FromBody] LoginCode loginCode)
        {
            try
            {
                var data = new[]
                {
                        new KeyValuePair<string, string>("client_id", graphAPISettings.Value.MS_User_Client_Id),
                        new KeyValuePair<string, string>("client_secret", graphAPISettings.Value.MS_User_Client_Secret),
                        new KeyValuePair<string, string>("grant_type", graphAPISettings.Value.MS_Grant_Type),
                        new KeyValuePair<string, string>("code", loginCode.Code),
                        new KeyValuePair<string, string>("resource", graphAPISettings.Value.MS_Resource),
                        new KeyValuePair<string, string>("redirect_uri", graphAPISettings.Value.MS_Redirect_Uri)
                };

                using (var httpClient = new HttpClient())
                {
                    using (var content = new FormUrlEncodedContent(data))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(graphAPISettings.Value.MS_TOKEN_URL, content);

                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MSToken>(response);

                        if (res.Access_Token != null)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(res.Access_Token);

                            var decodedData = JsonConvert.DeserializeObject<DecodedData>(JsonConvert.SerializeObject(token.Payload));

                            if (!_context.Users.Any(e => e.Email == decodedData.Preferred_username))
                            {
                                UserModel user = new()
                                {
                                    Email = decodedData.Preferred_username,
                                    FullName = decodedData.Name
                                };
                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();
                            }
                           
                            var loginResponse = new DigitalLoginResponse
                            {
                                Success = true,
                                Message = "Logged in successfully.",
                                Data = res,
                                DecodedData = token.Payload
                            };
                            return Ok(loginResponse);
                        }
                        else
                        {
                            var errorResponse = new DigitalFailureResponse
                            {
                                Success = false,
                                Message = "Failed to Get the Access Token"
                            };
                            return StatusCode(400, errorResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new DigitalFailureResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [Route("api/login/admin/code")]
        public async Task<ActionResult> AdminLoginTokenMethod([FromBody] LoginCode loginCode)
        {
            try
            {
                var data = new[]
                {
                        new KeyValuePair<string, string>("client_id", graphAPISettings.Value.MS_Admin_Client_Id),
                        new KeyValuePair<string, string>("client_secret", graphAPISettings.Value.MS_Admin_Client_Secret),
                        new KeyValuePair<string, string>("grant_type", graphAPISettings.Value.MS_Grant_Type),
                        new KeyValuePair<string, string>("code", loginCode.Code),
                        new KeyValuePair<string, string>("resource", graphAPISettings.Value.MS_Resource),
                        new KeyValuePair<string, string>("redirect_uri", graphAPISettings.Value.MS_Redirect_Uri)
                };

                using (var httpClient = new HttpClient())
                {
                    using (var content = new FormUrlEncodedContent(data))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(graphAPISettings.Value.MS_TOKEN_URL, content);

                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MSToken>(response);

                        if (res.Access_Token != null)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(res.Access_Token);

                            var decodedData = JsonConvert.DeserializeObject<DecodedData>(JsonConvert.SerializeObject(token.Payload));

                            if (!_context.Users.Any(e => e.Email == decodedData.Preferred_username))
                            {
                                UserModel user = new()
                                {
                                    Email = decodedData.Preferred_username,
                                    FullName = decodedData.Name
                                };
                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();
                            }

                            var loginResponse = new DigitalLoginResponse
                            {
                                Success = true,
                                Message = "Logged in successfully.",
                                Data = res,
                                DecodedData = token.Payload
                            };
                            return Ok(loginResponse);
                        }
                        else
                        {
                            var errorResponse = new DigitalFailureResponse
                            {
                                Success = false,
                                Message = "Failed to Get the Access Token"
                            };
                            return StatusCode(401, errorResponse);
                        }
                    }
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

        [HttpPost]
        [Route("api/login/user/refresh")]
        public async Task<ActionResult> UserRefreshAccessTokenMethod([FromBody] RefreshToken refreshToken)
        {
            try
            {
                var data = new[]
                {
                    new KeyValuePair<string, string>("client_id", graphAPISettings.Value.MS_User_Client_Id),
                    new KeyValuePair<string, string>("client_secret", graphAPISettings.Value.MS_User_Client_Secret),
                    new KeyValuePair<string, string>("grant_type", graphAPISettings.Value.MS_Refresh_Grant_Type),
                    new KeyValuePair<string, string>("refresh_token", refreshToken.refresh_token),
                    new KeyValuePair<string, string>("resource", graphAPISettings.Value.MS_Resource),
                    new KeyValuePair<string, string>("redirect_uri", graphAPISettings.Value.MS_Redirect_Uri)
                };

                using (var httpClient = new HttpClient())
                {
                    using (var content = new FormUrlEncodedContent(data))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(graphAPISettings.Value.MS_TOKEN_URL, content);

                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MSToken>(response);

                        if (res.Access_Token != null)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(res.Access_Token);

                            var decodedData = JsonConvert.DeserializeObject<DecodedData>(JsonConvert.SerializeObject(token.Payload));

                            if (!_context.Users.Any(e => e.Email == decodedData.Preferred_username))
                            {
                                UserModel user = new()
                                {
                                    Email = decodedData.Preferred_username,
                                    FullName = decodedData.Name,
                                    PhoneNumber = "",
                                    ProfilePicture = ""
                                };
                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();
                            }

                            var loginResponse = new DigitalLoginResponse
                            {
                                Success = true,
                                Message = "Logged in successfully.",
                                Data = res,
                                DecodedData = token.Payload,
                            };
                            return Ok(loginResponse);
                        }
                        else
                        {
                            var errorResponse = new DigitalFailureResponse
                            {
                                Success = false,
                                Message = "Failed to Get the Access Token"
                            };
                            return StatusCode(401, errorResponse);
                        }
                    }
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

        [HttpPost]
        [Route("api/login/admin/refresh")]
        public async Task<ActionResult> AdminRefreshAccessTokenMethod([FromBody] RefreshToken refreshToken)
        {
            try
            {
                var data = new[]
                {
                    new KeyValuePair<string, string>("client_id", graphAPISettings.Value.MS_Admin_Client_Id),
                    new KeyValuePair<string, string>("client_secret", graphAPISettings.Value.MS_Admin_Client_Secret),
                    new KeyValuePair<string, string>("grant_type", graphAPISettings.Value.MS_Refresh_Grant_Type),
                    new KeyValuePair<string, string>("refresh_token", refreshToken.refresh_token),
                    new KeyValuePair<string, string>("resource", graphAPISettings.Value.MS_Resource),
                    new KeyValuePair<string, string>("redirect_uri", graphAPISettings.Value.MS_Redirect_Uri)
                };

                using (var httpClient = new HttpClient())
                {
                    using (var content = new FormUrlEncodedContent(data))
                    {
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(graphAPISettings.Value.MS_TOKEN_URL, content);

                        var response = await httpResponseMessage.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<MSToken>(response);

                        if (res.Access_Token != null)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var token = handler.ReadJwtToken(res.Access_Token);

                            var decodedData = JsonConvert.DeserializeObject<DecodedData>(JsonConvert.SerializeObject(token.Payload));

                            if (!_context.Users.Any(e => e.Email == decodedData.Preferred_username))
                            {
                                UserModel user = new()
                                {
                                    Email = decodedData.Preferred_username,
                                    FullName = decodedData.Name,
                                    PhoneNumber = "",
                                    ProfilePicture = ""
                                };
                                _context.Users.Add(user);
                                await _context.SaveChangesAsync();
                            }

                            var loginResponse = new DigitalLoginResponse
                            {
                                Success = true,
                                Message = "Logged in successfully.",
                                Data = res,
                                DecodedData = token.Payload,
                            };
                            return Ok(loginResponse);
                        }
                        else
                        {
                            var errorResponse = new DigitalFailureResponse
                            {
                                Success = false,
                                Message = "Failed to Get the Access Token"
                            };
                            return StatusCode(401, errorResponse);
                        }
                    }
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
    }
}
