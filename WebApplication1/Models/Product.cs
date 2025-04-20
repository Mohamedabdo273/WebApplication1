namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Discription { get; set; }
        public string ImgUrl { get; set; }
        public int Count { get; set; }
        public string Model { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
