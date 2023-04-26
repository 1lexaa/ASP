namespace ASP.Models.User
{
    public class UserValidationModel
    {
        public String? NameMessage { get; set; }
        public String? LoginMessage { get; set; }
        public String? PasswordMessage { get; set; }
        public String? RepeatPasswordMessage { get; set; }
        public String? EmailMessage { get; set; }
        public String AvatarMessage { get; set; } = null!;
    }
}
