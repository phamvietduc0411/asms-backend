using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Model.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASMS.Services.Utilities
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        #region Employee Token
        public string GenerateEmployeeAccessToken(int employeeId, string email, string employeeRole)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, employeeRole),
                new Claim("Id", employeeId.ToString()),
                new Claim("Role", employeeRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                 audience: _configuration["Jwt:Audience"],
                 claims: claims,
                 expires: DateTime.UtcNow.AddHours(1),
                 signingCredentials: creds
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region Customer Token
        public string GenerateCustomerAccessToken(int customerId, string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("Id", customerId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                 audience: _configuration["Jwt:Audience"],
                 claims: claims,
                 expires: DateTime.UtcNow.AddHours(1),
                 signingCredentials: creds
             );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        public async Task<string> GenerateRefreshTokenAsync(int userId, bool isEmployee)
        {
            // crete new random token
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var entity = new RefreshToken
            {
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            if (isEmployee)
                entity.EmployeeId = userId;
            else
                entity.CustomerId = userId;

            await _unitOfWork.RefreshToken.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return refreshToken;
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var result = new AuthResponse();

            //find 
            var tokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);

            if (tokenEntity == null)
            {
                result.Success = false;
                result.ErrorMessage = "Refresh token invalid.";
                return result;
            }

            // check 
            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                result.Success = false;
                result.ErrorMessage = "Refresh token expires.";
                return result;
            }

            // check
            if (tokenEntity.RevokedAt != null)
            {
                result.Success = false;
                result.ErrorMessage = "Refresh token was revoked.";
                return result;
            }

            string newAccessToken = "";
            string newRefreshToken = "";

            // employee
            if (tokenEntity.EmployeeId != null)
            {
                var emp = await _unitOfWork.Employee.GetEntityByIdAsync(tokenEntity.EmployeeId.Value);
                if (emp == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Can not find employee.";
                    return result;
                }

                newAccessToken = GenerateEmployeeAccessToken(
                    emp.Id, emp.Username, emp.EmployeeRole.ToString()
                );

                newRefreshToken = await GenerateRefreshTokenAsync(emp.Id, true);
            }

            // customer
            if (tokenEntity.CustomerId != null)
            {
                var cus = await _unitOfWork.Customer.GetEntityByIdAsync(tokenEntity.CustomerId.Value);
                if (cus == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Can not find customer.";
                    return result;
                }

                newAccessToken = GenerateCustomerAccessToken(
                    cus.Id, cus.Email
                );

                newRefreshToken = await GenerateRefreshTokenAsync(cus.Id, false);
            }

            // Revoke old token 
            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshToken.UpdateAsync(tokenEntity);
            await _unitOfWork.CompleteAsync();

            result.Success = true;
            result.AccessToken = newAccessToken;
            result.RefreshToken = newRefreshToken;

            return result;
        }



    }
}
