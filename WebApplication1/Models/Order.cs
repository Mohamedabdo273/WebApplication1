namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get;set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public string UserID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}
