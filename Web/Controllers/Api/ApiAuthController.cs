using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class ApiAuthController : ControllerBase
{
    private IConfiguration configuration;

    public ApiAuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult Token(AdminLoginRequest request)
    {
        string adminPassword = configuration["Jwt:AdminPassword"];

        if (request.Password != adminPassword)
        {
            return Unauthorized();
        }

        string key = configuration["Jwt:Key"];
        string issuer = configuration["Jwt:Issuer"];
        string audience = configuration["Jwt:Audience"];
        int expiresMinutes = int.Parse(configuration["Jwt:ExpiresMinutes"]);

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long expires = DateTimeOffset.UtcNow.AddMinutes(expiresMinutes).ToUnixTimeSeconds();

        Dictionary<string, object> header = new Dictionary<string, object>();
        header["alg"] = "HS256";
        header["typ"] = "JWT";

        Dictionary<string, object> payload = new Dictionary<string, object>();
        payload["sub"] = "DesktopAdmin";
        payload["name"] = "DesktopAdmin";
        payload["role"] = "Admin";
        payload["iss"] = issuer;
        payload["aud"] = audience;
        payload["iat"] = now;
        payload["exp"] = expires;

        string unsignedToken = Base64Url(JsonSerializer.SerializeToUtf8Bytes(header)) + "." + Base64Url(JsonSerializer.SerializeToUtf8Bytes(payload));

        byte[] signatureBytes = new HMACSHA256(Encoding.UTF8.GetBytes(key)).ComputeHash(Encoding.UTF8.GetBytes(unsignedToken));

        string token = unsignedToken + "." + Base64Url(signatureBytes);

        return Ok(new { token });
    }

    private static string Base64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}

public class AdminLoginRequest
{
    public string Password { get; set; } = "admin";
}
