using ASP.Services.Hash;

namespace ASP.Services
{
    public class MD5HashService : IHashService
    {
        public String Hash(String text)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(text)));
        }
    }
}
