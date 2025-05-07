using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }          // مثال: iPhone 15 Pro Max
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string? Discription { get; set; }
        public string? ImgUrl { get; set; }
        public int Count { get; set; } //Stock

        public string? Model { get; set; }         // مثال: A3102
        public string? Brand { get; set; }         // مثال: Apple, HP, Samsung

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }


}
