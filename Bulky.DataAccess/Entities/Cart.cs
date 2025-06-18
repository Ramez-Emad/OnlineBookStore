namespace Bulky.DataAccess.Entities
{
    public class Cart
    {
        public string Id { get; set; } = default!;

        public ICollection<CartItem> Items { get; set; } = [];

        public double TotalCost { get; set; }
    }
}
