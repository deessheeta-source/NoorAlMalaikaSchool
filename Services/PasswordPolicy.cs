using System.Text.RegularExpressions;

namespace NoorAlMalaikaSchool.Services
{
    public static class PasswordPolicy
    {
        public static bool IsValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;
            return Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[a-z]")
                && Regex.IsMatch(password, "[0-9]")
                && Regex.IsMatch(password, "[^A-Za-z0-9]");
        }
    }
}
