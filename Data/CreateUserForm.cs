using System.ComponentModel.DataAnnotations;
using RedisBloomBlazor.Validators;

namespace RedisBloomBlazor.Data
{
    public class CreateUserForm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}