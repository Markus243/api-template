using System.Security.Cryptography;

namespace api_template.Middleware.Helpers
{
    public static class CodeGenerator
    {
        public static string GenerateCode(int length = 6)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            // Convert to a URL-friendly base32 string
            return Convert.ToBase64String(bytes)
                         .Replace("+", "")   .Replace("/", "")
                         .Replace("=", "")
                          [..length];
        }
    }
}

                      
