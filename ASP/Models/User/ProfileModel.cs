namespace ASP.Models.User
{
    public class ProfileModel
    {
        public ProfileModel()
        {
            Login = RealName = Email = null!;
        }
        public ProfileModel(DATA.Entity.User user)
        {
            var user_props = user.GetType().GetProperties();
            var this_props = this.GetType().GetProperties();

            foreach (var this_prop in this_props)
            {
                var prop = user_props.FirstOrDefault(p => p.Name == this_prop.Name && p.PropertyType.IsAssignableTo(this_prop.PropertyType));

                if (prop is not null)
                    this_prop.SetValue(this, prop.GetValue(user));
            }
        }

        public Guid Id { get; set; }
        public String Login { get; set; }
        public String RealName { get; set; }
        public String Email { get; set; }
        public String? Avatar { get; set; }
        public DateTime RegisterDt { get; set; }
        public DateTime? LastEnterDt { get; set; }
        public Boolean IsEmailPublic { get; set; }
        public Boolean IsRealNamePublick { get; set; }
        public Boolean IsDatesPublick { get; set; }
        public Boolean IsPersonal { get; set; }
    }
}
