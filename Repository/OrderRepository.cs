using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {
        public async Task<Order> AddOrderAsync(Order newOrder)
                => await OrderDAO.Instance.AddOrderAsync(newOrder);

        public async Task DeleteOrderAsync(int orderId)
                => await OrderDAO.Instance.DeleteOrderAsync(orderId);

        public async Task<Order> GetOrderAsync(int orderId)
                => await OrderDAO.Instance.GetOrderAsync(orderId);

        public async Task<IEnumerable<Order>> GetOrdersAsync()
                => await OrderDAO.Instance.GetOrdersAsync();

        public async Task<IEnumerable<Order>> GetOrdersAsync(int memberId)
                => await OrderDAO.Instance.GetOrdersAsync(memberId);

        public async Task<IEnumerable<Order>> SearchOrderAsync(DateTime startDate, DateTime endDate)
                => await OrderDAO.Instance.SearchOrderAsync(startDate, endDate);

        public async Task<Order> UpdateOrderAsync(Order updatedOrder)
                => await OrderDAO.Instance.UpdateOrderAsync(updatedOrder);
    }
}
