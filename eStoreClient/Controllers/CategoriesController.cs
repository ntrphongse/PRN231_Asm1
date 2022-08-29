using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using eStoreLibrary;
using System.Net.Http;

namespace eStoreClient.Controllers
{
    public class CategoriesController : Controller
    {
        public CategoriesController()
        {
        }

        // GET: Categories
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories");

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Category> categories = 
                        await response.Content.ReadAsAsync<IEnumerable<Category>>();
                    return View(categories);
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
                ViewData["Categories"] = ex.Message;
                return View();
            }
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Category is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Category category = await response.Content.ReadAsAsync<Category>();
                    return View(category);
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
                ViewData["Categories"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.POST,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories", category);

                    if (response.IsSuccessStatusCode)
                    {
                        Category createdCategory = await response.Content.ReadAsAsync<Category>();
                        if (createdCategory == null)
                        {
                            throw new Exception("Failed to create category!! Please check again...");
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
                    ViewData["Categories"] = ex.Message;
                    return View(category);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Category is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Category category = await response.Content.ReadAsAsync<Category>();
                    return View(category);
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
                ViewData["Categories"] = ex.Message;
                return View();
            }
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.PUT,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories/" + id, category);

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
                    ViewData["Categories"] = ex.Message;
                    return View(category);
                }
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Category is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Category category = await response.Content.ReadAsAsync<Category>();
                    return View(category);
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
                ViewData["Categories"] = ex.Message;
                return View();
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.DELETE,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories/" + id);

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
                ViewData["Categories"] = ex.Message;
                return View();
            }
        }
    }
}
