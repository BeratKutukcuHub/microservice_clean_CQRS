using System.Net.Mail;
using System.Text.RegularExpressions;
namespace IdentityService.Identity.Domain.Helper
{
    public class Checkers
    {
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                var _emailAddress = new MailAddress(emailaddress);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z]).{8,}$";
            bool isValid = Regex.IsMatch(password, pattern);
            return isValid;
        }
    }
}
