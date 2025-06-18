using Bulky.BL.Models.Categories;
using Bulky.DataAccess.Entities;

namespace Bulky.BL.Services.Orders
{
    public interface IOrderServices
    {
        Task CreateOrder(OrderHeader orderHeader , Cart cart, bool IsCompany);
        Task<OrderHeader> GetOrderHeaderByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByIdAsync(int id);
        Task<IEnumerable<OrderHeader>> GetAllOrdersAsync();
        Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);

        Task UpdateOrderHeader(OrderHeader orderHeader);
        Task ShipOrder(OrderHeader orderHeader);

    }
}
