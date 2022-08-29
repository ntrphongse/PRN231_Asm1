using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersAsync(int memberId);
        Task<IEnumerable<Order>> SearchOrderAsync(DateTime startDate, DateTime endDate);
        Task<Order> GetOrderAsync(int orderId);
        Task<Order> AddOrderAsync(Order newOrder);
        Task<Order> UpdateOrderAsync(Order updatedOrder);
        Task DeleteOrderAsync(int orderId);
    }
}
