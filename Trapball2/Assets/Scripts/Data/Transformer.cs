using System.Security.Cryptography;
using System.Text;

public static class Transformer
{
    private static string Key => Generator.GetKey();

    public static string Encode(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[16];
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return System.Convert.ToBase64String(encryptedBytes);
            }
        }
    }

    public static string Decode(string input)
    {
        byte[] inputBytes = System.Convert.FromBase64String(input);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = new byte[16];
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
