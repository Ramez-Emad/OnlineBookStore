namespace Bulky.DataAccess.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

    }
}
