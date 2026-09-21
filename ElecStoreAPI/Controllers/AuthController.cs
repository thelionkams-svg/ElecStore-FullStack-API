using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ElecStoreAPI.DTOs.Auth;

namespace ElecStoreAPI.Controllers
{

    [Route("api/[controller]")]

    [ApiController]

    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _config;


        // .NET بيجيب لينا الإعدادات من appsettings.json تلقائياً هنا
        public AuthController(IConfiguration config)
        {
            _config = config;
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string demoUsername = _config["DemoUser:Username"];
            string demoPasswordHash = _config["DemoUser:PasswordHash"];

            // 1. التحقق من اسم المستخدم
            if (request.Username != demoUsername)
                return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة");

            // 2. التحقق من كلمة المرور عن طريق مقارنتها بالـ Hash المخزّن
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, demoPasswordHash);

            if (!isPasswordValid)
                return Unauthorized("اسم المستخدم أو كلمة المرور غير صحيحة");

            // 3. لو الاتنين صح، نولّد توكن ونرجعه
            string token = GenerateJwtToken(demoUsername);

            return Ok(new { token = token });
        }


        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = double.Parse(_config["Jwt:ExpiryMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
    }

}
