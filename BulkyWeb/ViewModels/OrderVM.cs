using Bulky.DataAccess.Entities;

namespace BulkyWeb.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; } = default!;
        public IEnumerable<OrderDetail> OrderDetail { get; set; } = default!;
    }
}
