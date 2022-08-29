using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsAsync(int orderId)
                => await OrderDetailDAO.Instance.GetOrderDetailsAsync(orderId);
    }
}
