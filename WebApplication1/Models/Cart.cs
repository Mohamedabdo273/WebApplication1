namespace WebApplication1.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
