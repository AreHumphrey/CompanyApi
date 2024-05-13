using System;
using System.Security.Cryptography;

public static class SecurityUtils
{
    public static string GenerateSecureKey()
    {
        const int keySizeInBits = 256; // 256 bits for HS256 algorithm
        const int keySizeInBytes = keySizeInBits / 8;
        
        using (var random = new RNGCryptoServiceProvider())
        {
            var bytes = new byte[keySizeInBytes];
            random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}