using System.Security.Cryptography;
using System.Text;

namespace AuthorizationService
{
    public class PasswordEncryptor : IPasswordEncryptor
    {
        public string GenerateEncryptedPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string encryptedPassword, string password)
        {
            string hashedInputPassword = GenerateEncryptedPassword(password);
            return hashedInputPassword.Equals(encryptedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}

