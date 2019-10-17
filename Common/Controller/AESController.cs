using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Controller
{
    public class AESController
    {
        private static string password = "CryptoCode!";
        private readonly byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private const int BlockSize = 128;
        public string error { get; set; }

        public AESController()
        {
            if (string.IsNullOrEmpty(password))
                this.error = "El password de Encriptación es nulo";
            else
                this.error = null;
        }

        public bool HasError()
        {
            return !string.IsNullOrEmpty(this.error);
        }

        public string Encrypt(string texto)
        {
            try
            {
                texto = texto ?? "";

                byte[] bytes = Encoding.Unicode.GetBytes(texto);
                //Encrypt
                SymmetricAlgorithm crypt = Aes.Create();
                HashAlgorithm hash = MD5.Create();
                crypt.BlockSize = BlockSize;
                crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(password));
                crypt.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                       new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                this.error = "Error: " + ex.Message;
                return string.Empty;
            }
        }

        public string Decrypt(string texto)
        {
            try
            {
                texto = texto ?? "";

                //Decrypt
                byte[] bytes = Convert.FromBase64String(texto);
                SymmetricAlgorithm crypt = Aes.Create();
                HashAlgorithm hash = MD5.Create();
                crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(password));
                crypt.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    using (CryptoStream cryptoStream =
                       new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] decryptedBytes = new byte[bytes.Length];
                        cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                        return Encoding.Unicode.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {

                return "Error: " + ex.Message;
            }
        }
    }
}