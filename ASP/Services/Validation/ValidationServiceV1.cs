using System.Text.RegularExpressions;

namespace ASP.Services.Validation
{
    public class ValidationServiceV1 : IValidationService
    {
        public bool Validate(string source, params ValidationTerms[] terms)
        {
            if (terms.Length == 0)
                throw new NotImplementedException("No terms for valifator");
            if (terms.Length == 1 && terms[0] == ValidationTerms.None)
                return true;

            Boolean result = true;

            if (terms.Contains(ValidationTerms.NotEmpty))
                result &= !String.IsNullOrEmpty(source);  // result = result && x.
            if (terms.Contains(ValidationTerms.Email))
                result &= !ValidateEmail(source);
            if (terms.Contains(ValidationTerms.Login))
                result &= !ValidateLogin(source);
            if (terms.Contains(ValidationTerms.RealName))
                result &= !ValidateRealName(source);
            if (terms.Contains(ValidationTerms.Password))
                result &= !ValidatePassword(source);

            return result;
        }

        private static Boolean ValidateEmail(String source) { return ValidateRegex(source, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$"); }
        private static Boolean ValidateLogin(String source) { return ValidateRegex(source, @"^\w{3,}$"); }
        private static Boolean ValidateRealName(String source) { return ValidateRegex(source, @"^[a-zA-Zа-яА-Я]+([-'][a-zA-Zа-яА-Я]+)*$"); }
        private static Boolean ValidatePassword(String source) { return source.Length >= 3; }
        private static Boolean ValidateRegex(String source, String pattern) { return Regex.IsMatch(source, pattern); }
    }
}