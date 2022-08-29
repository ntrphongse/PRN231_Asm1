using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eStoreLibrary
{
    public static class eStoreClientUtils
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<HttpResponseMessage> ApiRequest(eStoreHttpMethod method, string apiUrl, object bodyExtra = null)
        {
            switch (method)
            {
                case eStoreHttpMethod.GET:
                    return await client.GetAsync(apiUrl);
                case eStoreHttpMethod.POST:
                    return await client.PostAsJsonAsync(apiUrl, bodyExtra);
                case eStoreHttpMethod.PUT:
                    return await client.PutAsJsonAsync(apiUrl, bodyExtra);
                case eStoreHttpMethod.DELETE:
                    return await client.DeleteAsync(apiUrl);
                default:
                    return null;
            }
        }

        public static bool IsAdmin(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }
            var claims = user.Claims;
            var role = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));
            if (role.Value.Equals("ADMIN"))
            {
                return true;
            }
            return false;
        }
    }
}
