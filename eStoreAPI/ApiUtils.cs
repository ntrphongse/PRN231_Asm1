using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eStoreAPI
{
    public static class ApiUtils
    {
        public static MemberRole GetMemberRole(this ClaimsPrincipal user)
        {
            return Enum.Parse<MemberRole>(user.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Role)).Value);
        }

        public static int GetMemberId(this ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value);
        }
    }
}
