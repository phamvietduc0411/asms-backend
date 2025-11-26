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
        public string GenerateEmployeeAccessToken(Employee employee)
        {
            var claims = GetEmployeeClaims(employee);

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
        public string GenerateCustomerAccessToken(Customer customer)
        {
            var claims = GetCustomerClaims(customer);

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

        public async Task<string> GenerateRefreshTokenAsync<T>(T user, bool isEmployee)
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
            {
                var emp = user as Employee;
                if (emp == null)
                    throw new Exception("User must be Employee when isEmployee = true");

                entity.EmployeeId = emp.Id;
            }
            else
            {
                var cus = user as Customer;
                if (cus == null)
                    throw new Exception("User must be Customer when isEmployee = false");

                entity.CustomerId = cus.Id;
            }

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

                newAccessToken = GenerateEmployeeAccessToken(emp);

                newRefreshToken = await GenerateRefreshTokenAsync(emp.Id, true);
            }

            // customer
            if (tokenEntity.CustomerId != null)
            {
                var customer = await _unitOfWork.Customer.GetEntityByIdAsync(tokenEntity.CustomerId.Value);
                if (customer == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Can not find customer.";
                    return result;
                }

                newAccessToken = GenerateCustomerAccessToken(customer);

                newRefreshToken = await GenerateRefreshTokenAsync(customer, false);
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

        public static List<Claim> GetEmployeeClaims(Employee emp)
        {
            return new List<Claim>
            {
                new ("Id", emp.Id.ToString()),
                new ("EmployeeCode", emp.EmployeeCode),
                new ("EmployeeRoleId", emp.EmployeeRoleId?.ToString() ?? ""),
                new ("Name", emp.Name ?? ""),
                new ("Phone", emp.Phone ?? ""),
                new ("Address", emp.Address ?? ""),
                new ("Username", emp.Username ?? ""),
                new ("Status", emp.Status ?? ""),
                new ("IsActive", emp.IsActive.ToString()),
            };
        }
        public static List<Claim> GetCustomerClaims(Customer cus)
        {
            return new List<Claim>
            {
                new ("Id", cus.Id.ToString()),
                new ("CustomerCode", cus.CustomerCode),
                new ("Name", cus.Name),
                new ("Phone", cus.Phone ?? ""),
                new ("Address", cus.Address ?? ""),
                new ("Email", cus.Email),
                new ("IsActive", cus.IsActive.ToString())
            };
        }
    }
}
