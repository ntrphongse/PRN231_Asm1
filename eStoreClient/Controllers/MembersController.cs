using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Net.Http;
using eStoreLibrary;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eStoreClient.Controllers
{
    public class MembersController : Controller
    {
        public MembersController()
        {
        }

        // GET: Members
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members");

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Member> members = await response.Content.ReadAsAsync<IEnumerable<Member>>();
                    return View(members);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Members"] = ex.Message;
                return View();
            }
        }

        //GET: Members/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member is not specified!");
                }
                string role = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Role)).Value;
                if (role.Equals(MemberRole.USER.ToString()))
                {
                    if (id != int.Parse(User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value))
                    {
                        return RedirectToAction("AccessDenied", "Login");
                    }
                }

                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Member member = await response.Content.ReadAsAsync<Member>();
                    return View(member);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Members"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,CompanyName,City,Country,Password")] Member member)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.POST,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members", member);

                    if (response.IsSuccessStatusCode)
                    {
                        Member createdMember = await response.Content.ReadAsAsync<Member>();
                        if (createdMember == null)
                        {
                            throw new Exception("Failed to create member!! Please check again...");
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("AccessDenied", "Login");
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Members"] = ex.Message;
                    return View(member);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member is not specified!");
                }
                string role = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Role)).Value;
                if (role.Equals(MemberRole.USER.ToString()))
                {
                    if (id != int.Parse(User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value))
                    {
                        return RedirectToAction("AccessDenied", "Login");
                    }
                }

                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Member member = await response.Content.ReadAsAsync<Member>();
                    return View(member);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Members"] = ex.Message;
                return View();
            }
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,Email,CompanyName,City,Country,Password")] Member member)
        {
            bool isAdmin = false;
            if (ModelState.IsValid)
            {
                try
                {
                    string role = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Role)).Value;
                    if (role.Equals(MemberRole.USER.ToString()))
                    {
                        if (id != int.Parse(User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value))
                        {
                            return RedirectToAction("AccessDenied", "Login");
                        }
                    } else
                    {
                        isAdmin = true;
                    }

                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.PUT,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/" + id, member);

                    if (response.IsSuccessStatusCode)
                    {
                        return isAdmin ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Details), new {id = id });
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("AccessDenied", "Login");
                    }
                    else
                    {
                        throw new Exception(await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Members"] = ex.Message;
                    return View(member);
                }
            }
            return View(member);
        }

        // GET: Members/Delete/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Member is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Member member = await response.Content.ReadAsAsync<Member>();
                    return View(member);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Members"] = ex.Message;
                return View();
            }
        }

        // POST: Members/Delete/5
        [Authorize(Roles = "ADMIN")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.DELETE,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Members/" + id);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                ViewData["Members"] = ex.Message;
                return View();
            }
        }

    }
}
