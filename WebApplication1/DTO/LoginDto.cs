using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class LoginDto
    {
        public int Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
