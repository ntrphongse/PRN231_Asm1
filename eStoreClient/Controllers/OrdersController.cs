using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using eStoreLibrary;
using System.Security.Claims;
using eStoreClient.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eStoreClient.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        public OrdersController()
        {
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpResponseMessage response;
                string fetchUrl = eStoreClientConfiguration.DefaultBaseApiUrl + "/Orders";
                
                if (!eStoreClientUtils.IsAdmin(User))
                {
                    string memberId = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                    fetchUrl += "/member/" + memberId;
                }

                response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    fetchUrl);

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Order> orders =
                        await response.Content.ReadAsAsync<IEnumerable<Order>>();
                    return View(orders);
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
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Report([FromQuery, BindRequired] DateTime? startDate, [FromQuery, BindRequired] DateTime? endDate)
        {
            try
            {
                if ((!startDate.HasValue || !endDate.HasValue)
                    && !(!startDate.HasValue && !endDate.HasValue)
                    )
                {
                    ViewData["SearchError"] = "Please input both start date and end date to generate a report!!";
                }

                ViewData["StartDate"] = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                ViewData["EndDate"] = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                HttpResponseMessage response;
                string fetchUrl = eStoreClientConfiguration.DefaultBaseApiUrl + 
                    "/Orders/search?startDate=" + startDate + "&endDate=" + endDate;

                //if (!eStoreClientUtils.IsAdmin(User))
                //{
                //    string memberId = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                //    fetchUrl += "/member/" + memberId;
                //}

                response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    fetchUrl);

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<Order> orders =
                        await response.Content.ReadAsAsync<IEnumerable<Order>>();
                    return View(orders);
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
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Order is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Orders/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Order order = await response.Content.ReadAsAsync<Order>();
                    return View(order);
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
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }

        private async Task<IEnumerable<Member>> GetMembersAsync()
        {
            HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.GET,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Members");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Member> members =
                    await response.Content.ReadAsAsync<IEnumerable<Member>>();
                return members;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Failed to get members! Please log out and log in again...");
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        
        private async Task<IEnumerable<Product>> GetProductsAsync()
        {
            HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.GET,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Products");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Product> products =
                    await response.Content.ReadAsAsync<IEnumerable<Product>>();
                return products;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Failed to get products! Please log out and log in again...");
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        [Authorize(Roles = "ADMIN")]
        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["ProductsSelectedList"] = new SelectList(await GetProductsAsync(), "ProductId", "ProductName");
                IEnumerable<Member> members =
                    await GetMembersAsync();
                ViewData["MemberId"] = new SelectList(members, "MemberId", "Email");
                ViewData["OrderDetails"] = HttpContext.Session.GetData<List<OrderDetail>>("OrderCart");

                return View();
            }
            catch (Exception ex)
            {
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] OrderDetailsModel order)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    IEnumerable<Member> members =
                        await GetMembersAsync();
                    ViewData["MemberId"] = new SelectList(members, "MemberId", "Email", order.MemberId);
                    ViewData["ProductsSelectedList"] = new SelectList(await GetProductsAsync(), "ProductId", "ProductName");
                    ViewData["OrderDetails"] = HttpContext.Session.GetData<List<OrderDetail>>("OrderCart");

                    IEnumerable<OrderDetail> orderDetails = HttpContext.Session.GetData<List<OrderDetail>>("OrderCart");
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        throw new Exception("Please add at least one item to your order details to create order");
                    }
                    order.OrderDetails = orderDetails.ToList();
                    foreach (var od in order.OrderDetails)
                    {
                        od.Product = null;
                    }
                    HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.POST,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Orders", order);

                    if (response.IsSuccessStatusCode)
                    {
                        Order createdOrder = await response.Content.ReadAsAsync<Order>();
                        if (createdOrder == null)
                        {
                            throw new Exception("Failed to create order!! Please check again...");
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
                    ViewData["Orders"] = ex.Message;
                    return View(order);
                }
                HttpContext.Session.SetData("OrderCart", null);
                return RedirectToAction(nameof(Index));
                
            }            
            return View(order);
        }

        private bool IsExistedInCart(IEnumerable<OrderDetail> cart, int productId)
        {
            return cart.Any(item => item.ProductId.Equals(productId));
        }

        private async Task<Product> GetProduct(int productId)
        {
            HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + productId);

            if (response.IsSuccessStatusCode)
            {
                Product product = await response.Content.ReadAsAsync<Product>();
                return product;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Failed to get products! Please log out and log in again...");
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddToCart([Bind("ProductId", "Quantity", "Discount")] OrderDetailsModel orderDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = await GetProduct(orderDetails.ProductId);
                    if (orderDetails.Quantity > product.UnitsInStock)
                    {
                        throw new Exception("Quantity must be less than " + product.UnitsInStock);
                    }
                    List<OrderDetail> cart = HttpContext.Session.GetData<List<OrderDetail>>("OrderCart");
                    if (cart == null)
                    {
                        cart = new List<OrderDetail>();
                        cart.Add(new OrderDetail
                        {
                            ProductId = orderDetails.ProductId,
                            Product = product,
                            UnitPrice = product.UnitPrice,
                            Quantity = orderDetails.Quantity,
                            Discount = orderDetails.Discount
                        });
                    }
                    else
                    {
                        if (IsExistedInCart(cart, orderDetails.ProductId))
                        {
                            int index = cart.FindIndex(od => od.ProductId.Equals(orderDetails.ProductId));
                            cart[index].Quantity += orderDetails.Quantity;
                        }
                        else
                        {
                            cart.Add(new OrderDetail
                            {
                                ProductId = orderDetails.ProductId,
                                Product = product,
                                UnitPrice = product.UnitPrice,
                                Quantity = orderDetails.Quantity,
                                Discount = orderDetails.Discount
                            });
                        }
                    }
                    HttpContext.Session.SetData("OrderCart", cart);
                    //ViewData["OrderDetails"] = cart;
                    //ViewData["ProductsSelectedList"] = new SelectList(await GetProductsAsync(), "ProductId", "ProductName", orderDetails.ProductId);
                    //ViewData["MemberId"] = new SelectList(await GetMembersAsync(), "MemberId", "Email");
                }
                return RedirectToAction(nameof(Create));
            } catch (Exception ex)
            {
                TempData["Orders"] = ex.Message;
                return RedirectToAction(nameof(Create));
            }
            
        }

        //// GET: Orders/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            throw new Exception("Order is not specified!");
        //        }
        //        HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
        //            eStoreHttpMethod.GET,
        //            eStoreClientConfiguration.DefaultBaseApiUrl + "/Products/" + id);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Product product = await response.Content.ReadAsAsync<Product>();
        //            ViewData["CategoryId"] = new SelectList(await GetCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);

        //            return View(product);
        //        }
        //        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //        {
        //            return RedirectToAction("AccessDenied", "Login");
        //        }
        //        else
        //        {
        //            throw new Exception(await response.Content.ReadAsStringAsync());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["Products"] = ex.Message;
        //        return View();
        //    }
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "City", order.MemberId);
        //    return View(order);
        //}

        //// POST: Orders/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        //{
        //    if (id != order.OrderId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.OrderId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "City", order.MemberId);
        //    return View(order);
        //}

        [Authorize(Roles = "ADMIN")]
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Order is not specified!");
                }
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                    eStoreHttpMethod.GET,
                    eStoreClientConfiguration.DefaultBaseApiUrl + "/Orders/" + id);

                if (response.IsSuccessStatusCode)
                {
                    Order order = await response.Content.ReadAsAsync<Order>();
                    return View(order);
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
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "ADMIN")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = await eStoreClientUtils.ApiRequest(
                eStoreHttpMethod.DELETE,
                eStoreClientConfiguration.DefaultBaseApiUrl + "/Orders/" + id);

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
                ViewData["Orders"] = ex.Message;
                return View();
            }
        }
    }
}
