using FactOfHuman.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace FactOfHuman.Extensions
{
    public static class GetUserIdFromClaims
    {
        public static Guid? getUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }
}
