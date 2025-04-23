namespace WebApplication1.Models
{
    public class ContactUS
    {
        public int Id { get;set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Reply { get; set; }
        public bool Status { get; set; }
        public string? UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
