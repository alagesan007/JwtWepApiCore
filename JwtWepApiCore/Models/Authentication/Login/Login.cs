using System.ComponentModel.DataAnnotations;

namespace JwtWepApiCore.Models.Authentication.Login
{
    public class Login
    {
        [Required(ErrorMessage ="Email Is Required")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage ="Password Is Required")]
        public string? Password { get; set; }
    }
}
