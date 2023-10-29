using System.Security.Cryptography;
using System.Text;

namespace AdminiDomain.Services
{
  /// <summary>
  /// User password encryption service.
  /// </summary>
  public static class CryptographyService
  {
    private const string AesKey = "U4R+7guw89F43HSsoxiAZA==";
    private const string IV = "NL1gbKF6gv7v4qD0d5J13Q==";

    private static Aes CreateCipher(string keyBase64)
    {
      Aes cipher = Aes.Create();
      cipher.Mode = CipherMode.CBC;
      cipher.Padding = PaddingMode.ISO10126;
      cipher.Key = Convert.FromBase64String(keyBase64);
      return cipher;
    }

    public static string Encrypt(string text)
    {
      string result;
      Aes cipher = CreateCipher(AesKey);
      try
      {
        cipher.IV = Convert.FromBase64String(IV);
        ICryptoTransform cryptTransform = cipher.CreateEncryptor();
        byte[] plaintext = Encoding.UTF8.GetBytes(text);
        byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);
        result = Convert.ToBase64String(cipherText);
      }
      catch (Exception)
      {
        return text;
      }
      return result;
    }

    public static string Decrypt(string encryptedText)
    {
      string result;
      Aes cipher = CreateCipher(AesKey);
      try
      {
        cipher.IV = Convert.FromBase64String(IV);
        ICryptoTransform cryptTransform = cipher.CreateDecryptor();
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        result = Encoding.UTF8.GetString(plainBytes);
      }
      catch (Exception)
      {
        return encryptedText;
      }
      return result;
    }
  }
}
