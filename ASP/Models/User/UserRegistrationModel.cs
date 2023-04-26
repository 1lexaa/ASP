namespace ASP.Models.User
{
    public class UserRegistrationModel
    {
        public String? Name { get; set; }
        public String? Login { get; set; }
        public String? Password { get; set; }
        public String? RepeatPassword { get; set; }
        public String? Email { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
