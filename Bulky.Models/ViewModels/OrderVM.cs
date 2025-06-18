namespace Bulky.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; } = default!;
        public IEnumerable<OrderDetail> OrderDetail { get; set; } = default!;
    }
}
