namespace Bulky.DataAccess.Entities

{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
