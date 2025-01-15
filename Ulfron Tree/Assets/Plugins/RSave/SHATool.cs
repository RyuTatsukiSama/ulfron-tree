using System.Security.Cryptography;
using System.Text;

public static class SHATool
{

    public static string GenerateSHA512String(string inputString)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(inputString); 
        byte[] hash = SHA512.Create().ComputeHash(bytes);
        return GetStringFromHash(hash);
    }

    public static string GenerateSHA256String(string inputString)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(inputString);
        byte[] hash = SHA256.Create().ComputeHash(bytes);
        return GetStringFromHash(hash);
    }

    public static string GetStringFromHash(byte[] hash)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for(int i= 0; i<hash.Length; i++)
        {
            stringBuilder.Append(hash[i].ToString("X2"));
        }
        return stringBuilder.ToString();
    }
}