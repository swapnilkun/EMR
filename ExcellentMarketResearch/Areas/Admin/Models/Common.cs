using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models
{
    public static class Common
    {

        public static string MySession()
        {
            string s_user = HttpContext.Current.Session["userid"].ToString();
            return s_user;
        }

        public static string GenerateSlug(this string phrase)
        {
            phrase = phrase.Replace(" -", " ");
            phrase = phrase.Replace("- ", " ");
            phrase = phrase.Replace(" - ", " ");

            phrase = phrase.Replace("-", " ");

            string str = Regex.Replace(phrase, @"[^A-Za-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ");
            string[] words = str.Split(' ');
            string[] validwords = new String[8];

            if (words.Length <= 8)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    validwords[i] = words[i];
                    str = string.Join(" ", validwords).Trim();
                    str = Regex.Replace(str, @"\s+", "-");
                }

            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    validwords[i] = words[i];
                    str = string.Join(" ", validwords).Trim();
                    str = Regex.Replace(str, @"\s+", "-");
                }
            }

            return str.ToLower();

        }


        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}