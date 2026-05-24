using System;
using System.Security.Cryptography;
using System.Text;

namespace BUS
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(hashedPassword)) return false;
            
            string hashOfInput = HashPassword(inputPassword);
            return hashOfInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
