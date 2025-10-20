using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RTS.SharedKernel.Extensions
{
    public static class HttpContextExtensions
    {
        public static int? GetCurrentUserId(this HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
                return int.Parse(userIdClaim.Value);
            return null;
        }

        public static string GetToken(this HttpContext context) =>
            context.Request.Headers.Authorization.ToString().Replace($"{JwtBearerDefaults.AuthenticationScheme} ", string.Empty);
    }
}
