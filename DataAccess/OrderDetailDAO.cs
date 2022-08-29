using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;
        private readonly static object instanceLock = new object();
        private OrderDetailDAO()
        {

        }
        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            var database = new FStoreContext();
            return await database.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product)
                .ToListAsync();
        }

        //public OrderDetail Add(OrderDetail newOrderDetail)
        //{
        //    OrderDetail orderDetail = null;
        //    var database = new FStoreContext();
        //    database.OrderDetails.Add(newOrderDetail);
        //    database.SaveChanges();
        //    orderDetail = newOrderDetail;
        //    return orderDetail;
        //}

        //public OrderDetail GetOrderDetail(int orderId, int productId)
        //{
        //    OrderDetail orderDetail = null;
        //    var database = new FStoreContext();
        //    orderDetail = database.OrderDetails
        //        .Include(od => od.Product)
        //        .SingleOrDefault(od => od.OrderId == orderId
        //                        && od.ProductId == productId);
        //    return orderDetail;
        //}
        //public void Delete(int orderId, int productId)
        //{
        //    if (GetOrderDetail(orderId, productId) == null)
        //    {
        //        throw new Exception("Order Detail does not exist!!");
        //    }

        //    var database = new FStoreContext();
        //    database.OrderDetails.Remove(
        //        GetOrderDetail(orderId, productId));
        //    database.SaveChanges();
        //}

        //public OrderDetail Update(OrderDetail updatedOrderDetail)
        //{
        //    OrderDetail orderDetail = null;
        //    if (GetOrderDetail(updatedOrderDetail.OrderId, updatedOrderDetail.ProductId) == null)
        //    {
        //        throw new Exception("Order Detail does not exist!!");
        //    }
        //    var database = new FStoreContext();
        //    database.OrderDetails.Update(updatedOrderDetail);
        //    database.SaveChanges();
        //    orderDetail = updatedOrderDetail;
        //    return orderDetail;
        //}
    }
}
