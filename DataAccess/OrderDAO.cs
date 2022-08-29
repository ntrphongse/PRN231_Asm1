using BusinessObject;
using eStoreLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDAO
    {
        private static OrderDAO instance = null;
        private static readonly object instanceLock = new object();
        private OrderDAO()
        {

        }
        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var database = new FStoreContext();
            IEnumerable<Order> orders = await database.Orders
                .Include(order => order.Member)
                .Include(order => order.OrderDetails)
                .ToListAsync();
            foreach (Order order in orders)
            {
                if (order.Member != null)
                {
                    if (order.Member.Orders != null && order.Member.Orders.Any())
                    {
                        order.Member.Orders = null;
                    }
                }
            }
            return orders;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(int memberId)
        {
            var database = new FStoreContext();
            IEnumerable<Order> orders = await database.Orders.Where(order => order.MemberId == memberId)
                .Include(order => order.Member)
                .Include(order => order.OrderDetails)
                .ToListAsync();
            foreach (Order order in orders)
            {
                if (order.Member != null)
                {
                    if (order.Member.Orders != null && order.Member.Orders.Any())
                    {
                        order.Member.Orders = null;
                    }
                }
            }
            return orders;
        }

        public async Task<IEnumerable<Order>> SearchOrderAsync(DateTime startDate, DateTime endDate)
        {
            var database = new FStoreContext();
            IEnumerable<Order> orders = await database.Orders
                .Where(order => DateTime.Compare(order.OrderDate, startDate) >= 0 &&
                        DateTime.Compare(order.OrderDate, endDate) <= 0)
                .Include(order => order.Member)
                .Include(order => order.OrderDetails)
                .ToListAsync();
            foreach (Order order in orders)
            {
                if (order.Member != null)
                {
                    if (order.Member.Orders != null && order.Member.Orders.Any())
                    {
                        order.Member.Orders = null;
                    }
                }
            }
            return orders;
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            var database = new FStoreContext();
            return await database.Orders
                .Include(order => order.Member)
                .Include(order => order.OrderDetails)
                .ThenInclude(od => od.Product)
                .SingleOrDefaultAsync(or => or.OrderId == orderId);
        }

        public async Task<Order> AddOrderAsync(Order newOrder)
        {
            await CheckOrder(newOrder);
            var database = new FStoreContext();
            newOrder.OrderId = await GetNextOderIdAsync();
            newOrder.OrderDate = DateTime.Now;
            if (newOrder.OrderDetails != null && newOrder.OrderDetails.Any())
            {
                foreach (var od in newOrder.OrderDetails)
                {
                    Product product = await database.Products.FindAsync(od.ProductId);
                    if (product.UnitsInStock < od.Quantity)
                    {
                        throw new ApplicationException("Order Quantity of '" + product.ProductName
                            + "' is more than the units in stock! Please check again!!");
                    }
                    product.UnitsInStock -= od.Quantity;
                    database.Products.Update(product);
                }
            }
            await database.Orders.AddAsync(newOrder);
            
             await database.SaveChangesAsync();
            return newOrder;
        }

        public async Task<Order> UpdateOrderAsync(Order updatedOrder)
        {
            if (await GetOrderAsync(updatedOrder.OrderId) == null)
            {
                throw new ApplicationException($"The order with the ID {updatedOrder.OrderId} does not exist! " +
                    $"Please check with the developer for more information...");
            }
            await CheckOrder(updatedOrder);
            var database = new FStoreContext();
            database.Orders.Update(updatedOrder);
            await database.SaveChangesAsync();
            return updatedOrder;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            Order deletedOrder = await GetOrderAsync(orderId);
            if (deletedOrder == null)
            {
                throw new ApplicationException($"The order with the ID {orderId} does not exist! Please check again...");
            }
            var database = new FStoreContext();
            database.Orders.Remove(deletedOrder);
            await database.SaveChangesAsync();
        }

        private async Task<int> GetNextOderIdAsync()
        {
            var database = new FStoreContext();
            return await database.Orders.MaxAsync(or => or.OrderId) + 1;
        }

        private async Task CheckOrder(Order order)
        {
            var database = new FStoreContext();
            if (await database.Members.FindAsync(order.MemberId) == null)
            {
                throw new ApplicationException("Member is not existed!!");
            }
            if (order.RequiredDate.HasValue && order.RequiredDate.Value < order.OrderDate)
            {
                throw new ApplicationException("Order Required Date has to later than Order Date");
            }
            if (order.ShippedDate.HasValue && order.ShippedDate.Value < order.OrderDate)
            {
                throw new ApplicationException("Order Shipped Date has to later than Order Date");
            }
            if (order.Freight.HasValue)
            {
                order.Freight.Value.DecimalValidate(minimum: 0,
                    minErrorMessage: "Order Freight has to be a positive number!!",
                    maximum: decimal.MaxValue,
                    maxErrorMessage: $"Order Freight is limited to the value of {decimal.MaxValue}!");
            }
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (OrderDetail orderDetail in order.OrderDetails)
                {
                    if (await database.Products.FindAsync(orderDetail.ProductId) == null)
                    {
                        throw new ApplicationException($"Product with the ID {orderDetail.ProductId} is not existed!!");
                    }
                    orderDetail.UnitPrice.DecimalValidate(minimum: 0,
                        minErrorMessage: "Order detail Unit Price has to be a positive number!!",
                        maximum: decimal.MaxValue,
                        maxErrorMessage: $"Order detail Unit Price is limited to the value of {decimal.MaxValue}!");

                    orderDetail.Quantity.IntegerValidate(minimum: 0,
                        minErrorMessage: "Order detail Quantity has to be a positive number!!",
                        maximum: int.MaxValue,
                        maxErrorMessage: $"Order detail Quantity is limited to the value of {int.MaxValue}!");

                    orderDetail.Discount.DoubleValidate(minimum: 0,
                        minErrorMessage: "Order detail Discount has to be a positive number!!",
                        maximum: double.MaxValue,
                        maxErrorMessage: $"Order detail Discount is limited to the value of {double.MaxValue}!");
                }
                
            }
        }
    }
}
