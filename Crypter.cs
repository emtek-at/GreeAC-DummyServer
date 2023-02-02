using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DummyServer
{
    public class Crypter
    {
        static string m_Key = "a3K8Bx%2r8Y7#xDh";

        public static string Decrypt(string pack, string key)
        {
            if (key == "")
                key = Crypter.m_Key;
            
            var myaes = Aes.Create();
            myaes.Mode = CipherMode.ECB;
            myaes.Key = Encoding.UTF8.GetBytes(key);
            myaes.Padding = PaddingMode.PKCS7;
            myaes.GenerateIV();

            var decryptor = myaes.CreateDecryptor(myaes.Key, myaes.IV);
            var msDecrypt = new MemoryStream(Convert.FromBase64String(pack));
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            var srDecrypt = new StreamReader(csDecrypt);
            var plaintext = srDecrypt.ReadToEnd();
            
            return plaintext;
        }

        public static string Encrypt(string pack, string key)
        {
            if (key == "")
                key = Crypter.m_Key;

            var myaes = Aes.Create();
            myaes.Mode = CipherMode.ECB;
            myaes.Key = Encoding.UTF8.GetBytes(key);
            myaes.Padding = PaddingMode.PKCS7;
            myaes.GenerateIV();

            var encryptor = myaes.CreateEncryptor(myaes.Key, myaes.IV);
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(pack);
            swEncrypt.Flush();
            swEncrypt.Close();
            msEncrypt.Flush();
            var encrypted = Convert.ToBase64String(msEncrypt.ToArray());

            return encrypted;
        }
    }
}