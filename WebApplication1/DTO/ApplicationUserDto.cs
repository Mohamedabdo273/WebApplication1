using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get;set; }
    }
}
