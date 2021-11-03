using System.ComponentModel.DataAnnotations;

namespace testRedisBloom.Data
{
    public class LoginForm
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}