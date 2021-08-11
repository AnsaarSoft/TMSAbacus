using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TMSDeloitte.Helper
{
    public class Encrypt_Decrypt
    {
        public string key = "";
        public Encrypt_Decrypt()
        {
            key = "b14ca5898a4e4133bbce2ea2315a1916";
        }


        public string EncryptString(string plainText)
        {
            string val = "";
            try {
                
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                val= Convert.ToBase64String(array);
            }
            catch(Exception)
            {

            }
            return val;
        }

        public string DecryptString(string cipherText)
        {
            string val = "";
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                val = streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("Encrypt_Decrypt", "Exception occured on methond of DecryptString in Encrypt_Decrypt.cs Class, " + ex.Message);
            }
            return val;
        }


        public string EncryptURLString(string plainText)
        {
            //string val = "";
            //try
            //{
            //    byte[] iv = new byte[16];
            //    byte[] array;

            //    using (Aes aes = Aes.Create())
            //    {
            //        aes.Key = Encoding.UTF8.GetBytes(key);
            //        aes.IV = iv;

            //        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //        using (MemoryStream memoryStream = new MemoryStream())
            //        {
            //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
            //            {
            //                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
            //                {
            //                    streamWriter.Write(plainText);
            //                }

            //                array = memoryStream.ToArray();
            //            }
            //        }
            //    }

            //    val = Convert.ToString(Convert.ToBase64String(array)).Replace("+", "_");
            //}
            //catch(Exception)
            //{

            //}
            return plainText;
            //return val;
          
        }

        public string DecryptURLString(string cipherText)
        {
            //string val = "";
            //cipherText=cipherText.Replace("_", "+");
            //try
            //{
            //    byte[] iv = new byte[16];
            //    byte[] buffer = Convert.FromBase64String(cipherText);

            //    using (Aes aes = Aes.Create())
            //    {
            //        aes.Key = Encoding.UTF8.GetBytes(key);
            //        aes.IV = iv;
            //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //        using (MemoryStream memoryStream = new MemoryStream(buffer))
            //        {
            //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
            //            {
            //                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
            //                {
            //                    val= streamReader.ReadToEnd();
            //                }
            //            }
            //        }
            //    }

            //}
            //catch(Exception)
            //{

            //}
            //return val;
            return cipherText;
        }

    }
}