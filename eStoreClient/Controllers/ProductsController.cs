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

namespace eStoreClient.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class ProductsController : Controller
    {

        public ProductsController()
        {
        }

        // GET: Products
        public async Task<IActionResult> Index([FromQuery] string searchString, 
            [FromQuery] decimal? startPrice, [FromQuery] decimal? endPrice)
        {
            try
            {
                HttpResponseMessage response;
                string fetchUrl = eStoreClientConfiguration.DefaultBaseApiUrl + "/Products";
                if ((!string.IsNullOrEmpty(searchString) && (startPrice.HasValue || endPrice.HasValue))
                    || (string.IsNullOrEmpty(searchString) && (!startPrice.HasValue || !endPrice.HasValue))
                    && !(string.IsNullOrEmpty(searchString) && !startPrice.HasValue && !endPrice.HasValue))
                {
                    ViewData["SearchError"] = "Invalid search or filter!!";
                    return View();
                }
                if (!string.IsNullOrEmpty(searchString) && !startPrice.HasValue && !endPrice.HasValue)
                {
                    ViewData["search"] = searchString;
                    fetchUrl += "/search?searchKeyword=" + searchString;
                    
                } else if (startPrice.HasValue && endPrice.HasValue)
                {
                    ViewData["StartPrice"] = startPrice.Value;
                    ViewData["EndPrice"] = endPrice.Value;
                    fetchUrl += "/filter?startPrice=" + startPrice + "&endPrice=" + endPrice;
                } else
                {
                    
                }
                response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    fetchUrl);

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Product> products =
                        await response.Content.ReadAsAsync<IEnumerable<Product>>();
                    return View(products);
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
                ViewData["Products"] = ex.Message;
                return View();
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Product is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Product product = await response.Content.ReadAsAsync<Product>();
                    return View(product);
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
                ViewData["Products"] = ex.Message;
                return View();
            }
        }

        private async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.GET,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Categories");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Category> categories =
                    await response.Content.ReadAsAsync<IEnumerable<Category>>();
                return categories;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Failed to get categories! Please log out and log in again...");
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                IEnumerable<Category> categories =
                    await GetCategoriesAsync();
                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Products"] = ex.Message;
                return View();
            }
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,ProductName,Weight,UnitPrice,UnitsInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IEnumerable<Category> categories =
                        await GetCategoriesAsync();
                    ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName");

                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.POST,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products", product);

                    if (response.IsSuccessStatusCode)
                    {
                        Product createdProduct = await response.Content.ReadAsAsync<Product>();
                        if (createdProduct == null)
                        {
                            throw new Exception("Failed to create product!! Please check again...");
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
                    ViewData["Products"] = ex.Message;
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Product is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Product product = await response.Content.ReadAsAsync<Product>();
                    ViewData["CategoryId"] = new SelectList(await GetCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);

                    return View(product);
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
                ViewData["Products"] = ex.Message;
                return View();
            }
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,CategoryId,ProductName,Weight,UnitPrice,UnitsInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.PUT,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id, product);

                    ViewData["CategoryId"] = new SelectList(await GetCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);

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
                    ViewData["Product"] = ex.Message;
                    return View(product);
                }
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Product is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Product product = await response.Content.ReadAsAsync<Product>();
                    return View(product);
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
                ViewData["Products"] = ex.Message;
                return View();
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.DELETE,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id);

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
                ViewData["Products"] = ex.Message;
                return View();
            }
        }
    }
}
