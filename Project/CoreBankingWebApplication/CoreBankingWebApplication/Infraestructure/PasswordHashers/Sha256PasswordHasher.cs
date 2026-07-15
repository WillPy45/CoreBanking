using System.Security.Cryptography;
using System.Text;


namespace CoreBankingWebApplication.Infraestructure.PasswordHashers
{
    public class Sha256PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedPassword = SHA256.HashData(bytes);

            return Convert.ToBase64String(hashedPassword);
        }

        public bool Verify(string hashedPassword, string inputPassword)
        {
            return HashPassword(inputPassword) == hashedPassword;
        }
    }
}
