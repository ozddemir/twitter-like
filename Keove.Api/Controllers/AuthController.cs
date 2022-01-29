using Keove.DataAccess.IRepository;
using Keove.Dto.Auth;
using Keove.Dto.Configuration;
using Keove.Dto.Responses;
using Keove.Entity.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Keove.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private RoleManager<ApplicationRole> _roleManager;
        public AuthController(UserManager<ApplicationUser> userManager,
            IOptionsMonitor<JwtConfig> optionMonitor,
            TokenValidationParameters tokenValidationParameters,
            IRefreshTokenRepository refreshTokenRepo,
            RoleManager<ApplicationRole> roleManager
            )
        {
            _userManager = userManager;
            _jwtConfig = optionMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
            _refreshTokenRepo = refreshTokenRepo;
            _roleManager = roleManager;
        }


        //[HttpPost("role")]
        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = "Guest" });
        //    return null;
        //}


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (ModelState.IsValid)
            {
                const string roleName = "User";

                try
                {

                    var userExist = await _userManager.FindByEmailAsync(dto.Email);

                    if (userExist != null)
                    {
                        return BadRequest(new ErrorResponseDto()
                        {
                            Success = false,
                            Errors = new List<string>()
                        {
                            "The Email is already in use"
                        }
                        });
                    }

                    // adding user
                    var appUser = new ApplicationUser
                    {
                        UserName = dto.UserName,
                        Email = dto.Email
                    };

                    IdentityResult isCreated = await _userManager.CreateAsync(appUser, dto.Password);
                    if (!isCreated.Succeeded)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Errors = isCreated.Errors.Select(x => x.Description).ToList()
                        });
                    }
                    
                    // Check if the role exist
                    var roleExist = await _roleManager.RoleExistsAsync(roleName);

                    if (!roleExist) // checks on the role exist status
                    {
                        return BadRequest(new
                        {
                            error = "Role does not exist"
                        });
                    }

                    var result = await _userManager.AddToRoleAsync(appUser, roleName);

                    if (!result.Succeeded)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Errors = new List<string>()
                        {
                             "Role is not created"
                        }
                        });
                    }


                    // create a jtw token
                    var token = await GenerateJwtToken(appUser);

                    if (token.Success == true)
                    {
                        return Ok(new AuthResponseDto
                        {
                            Success = token.Success,
                            Token = token.Token,
                            RefreshToken = token.RefreshToken,
                            Email = appUser.Email,
                            UserName = appUser.UserName
                        });
                    }
                }
                catch (Exception)
                {

                    return BadRequest(new ErrorResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                             "The model is not valid"
                        }
                    });
                }

            }
            return BadRequest(new ErrorResponseDto
            {
                Success = false,
                Errors = new List<string>()
                        {
                             "The model is not valid"
                        }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(dto.Email);

                if (userExist == null)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                             "Email or password is incorrect"
                        }
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(userExist, dto.Password);
                if (isCorrect)
                {
                    var token = await GenerateJwtToken(userExist);
                    if (token.Success)
                    {
                        return Ok(new AuthResponseDto
                        {
                            Success = token.Success,
                            Token = token.Token,
                            RefreshToken = token.RefreshToken,
                            Email = userExist.Email,
                            UserName = userExist.UserName
                        });
                    }
                    return BadRequest(new ErrorResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                             "Not able to create a token"
                        }
                    });

                }
                else
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                             "Email or password is incorrect"
                        }
                    });
                }
            }

            return BadRequest(new ErrorResponseDto
            {
                Success = false,
                Errors = new List<string>()
                    {
                        "Email and password are required"
                    }
            });

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(dto);

                if (result == null)
                {
                    return BadRequest(new ErrorResponseDto()
                    {
                        Errors = new List<string>() {
                            "Invalid tokens"
                        },
                        Success = false
                    });
                }
                return Ok(result);

            }
            return BadRequest(new ErrorResponseDto
            {
                Success = false,
                Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
            });
        }

        [HttpPost("guest-register")]
        public async Task<IActionResult> GuestRegister([FromBody] UserRegisterDto dto)
        {
            if (ModelState.IsValid)
            {
                const string roleName = "Guest";

                try
                {

                    var userExist = await _userManager.FindByEmailAsync(dto.Email);

                    if (userExist != null)
                    {
                        return BadRequest(new ErrorResponseDto()
                        {
                            Success = false,
                            Errors = new List<string>()
                        {
                            "The Email is already in use"
                        }
                        });
                    }

                    // adding user
                    var appUser = new ApplicationUser
                    {
                        UserName = dto.UserName,
                        Email = dto.Email
                    };

                    IdentityResult isCreated = await _userManager.CreateAsync(appUser, dto.Password);
                    if (!isCreated.Succeeded)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Errors = isCreated.Errors.Select(x => x.Description).ToList()
                        });
                    }

                    // Check if the role exist
                    var roleExist = await _roleManager.RoleExistsAsync(roleName);

                    if (!roleExist) // checks on the role exist status
                    {
                        return BadRequest(new
                        {
                            error = "Role does not exist"
                        });
                    }

                    var result = await _userManager.AddToRoleAsync(appUser, roleName);

                    if (!result.Succeeded)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Errors = new List<string>()
                        {
                             "Role is not created"
                        }
                        });
                    }


                    // create a jtw token
                    var token = await GenerateJwtToken(appUser);

                    if (token.Success == true)
                    {
                        return Ok(new AuthResponseDto
                        {
                            Success = token.Success,
                            Token = token.Token,
                            RefreshToken = token.RefreshToken,
                            Email = appUser.Email,
                            UserName = appUser.UserName
                        });
                    }
                }
                catch (Exception)
                {

                    return BadRequest(new ErrorResponseDto
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                             "The model is not valid"
                        }
                    });
                }

            }
            return BadRequest(new ErrorResponseDto
            {
                Success = false,
                Errors = new List<string>()
                        {
                             "The model is not valid"
                        }
            });


        }

        private async Task<TokenResponseDto> GenerateJwtToken(ApplicationUser user)
        {
            // responsible for creating the  token
            var jwtHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            //generate the security obj token
            var token = jwtHandler.CreateToken(tokenDescriptor);

            //convert the security obj token into a string
            var jwtToken = jwtHandler.WriteToken(token);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                AddDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddHours(24),
                Token = GenerateRefreshToken()
            };

            try
            {
                //refactor 
                await _refreshTokenRepo.UpsertRefreshTokenAsync(refreshToken);

                return new TokenResponseDto
                {
                    Token = jwtToken,
                    Success = true,
                    RefreshToken = refreshToken.Token
                };


            }
            catch (Exception ex)
            {

                return new TokenResponseDto
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Something went wrong"
                    }
                };
            }
        }

        private async Task<TokenResponseDto> VerifyAndGenerateToken(TokenRequestDto dto)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false; // prevent catch 
                var tokenInVerification = jwtTokenHandler.ValidateToken(dto.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }
                // check expiry date of the token
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    //token has not yet expired
                    return new TokenResponseDto
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }


                var storedRefreshToken = await _refreshTokenRepo.FindByRefreshTokenAsync(dto.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }

                if (storedRefreshToken.IsUsed)
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }


                if (storedRefreshToken.IsRevoked)
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }

                if (storedRefreshToken.ExpiryDate <= DateTime.UtcNow)
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Refreh Token has been expired. Please re-login "
                        }
                    };
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedRefreshToken.JwtId != jti)
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }

                //update current refreshToken
                storedRefreshToken.IsUsed = true;

                await _refreshTokenRepo.UpdateAsync(storedRefreshToken);

                //generate new token for the user
                var appUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
                _tokenValidationParameters.ValidateLifetime = true; // prevent to catch ex
                return await GenerateJwtToken(appUser);

            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {

                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new TokenResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }


        // Get all valid claims for the corresponding user
        private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Getting the claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Get the user role and add it to the claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));

                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
