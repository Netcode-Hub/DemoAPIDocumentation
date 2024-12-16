using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoAPIDocumentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(IConfiguration config) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet(Name = "GetWeatherForecast")]
        [EndpointSummary("Weather forecast")]
        [EndpointDescription("This gets us weather forecast")]
        [ProducesResponseType(200)]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("private/{number:int}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPrivate(int number)
        {
            return number > 6 ? Ok(number) : BadRequest(number);
        }

        [HttpGet("getToken")]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public IActionResult GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Netcode-Hub"),
                new Claim(ClaimTypes.Email, "netcode@hub.com")
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
