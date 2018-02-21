using System;
using System.Security.Claims;

namespace UserManagement.Utilities
{
    public static class Utilities
    {
        public static int GetCurrentUserId(ClaimsPrincipal user)
        {
            Int32.TryParse(user.FindFirst("jti")?.Value, out int id);

            return id;
        }
    }
}
