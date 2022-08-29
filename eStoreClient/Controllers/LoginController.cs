using BusinessObject;
using eStoreClient.Models;
using eStoreLibrary;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eStoreClient.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        [AllowAnonymous]
        public IActionResult Index([FromQuery] string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                if (eStoreClientUtils.IsAdmin(User))
                {
                    return RedirectToAction("Index", "Members");
                } else
                {
                    return RedirectToAction("Index", "Orders");
                }
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm, Bind("Email", "Password")] MemberLoginModel memberLoginInfo,
                                                [FromForm, Bind("ReturnUrl")] string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                if (eStoreClientUtils.IsAdmin(User))
                {
                    return RedirectToAction("Index", "Members");
                }
                else
                {
                    return RedirectToAction("Index", "Orders");
                }
            }
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.POST,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/login",
                memberLoginInfo);

                if (response.IsSuccessStatusCode)
                {
                    MemberWithRole loginMember = await response.Content.ReadAsAsync<MemberWithRole>();
                    if (loginMember == null)
                    {
                        throw new Exception("Failed to login! Please check again...");
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, loginMember.Email),
                        new Claim(ClaimTypes.NameIdentifier, loginMember.MemberId.ToString()),
                        new Claim(ClaimTypes.Role, loginMember.MemberRoleString)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var memberPrincipal = new ClaimsPrincipal(new[] { claimsIdentity });

                    await HttpContext.SignInAsync(memberPrincipal);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        if (eStoreClientUtils.IsAdmin(User))
                        {
                            return RedirectToAction("Index", "Members");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Orders");
                        }
                    }
                    return Redirect(returnUrl);

                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Login"] = ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult AccessDenied([FromQuery] string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }
    }
}
