namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get;set; }
        public int Count { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int ProductID { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending; 

        public Product? Product { get; set; }
        public string? UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
