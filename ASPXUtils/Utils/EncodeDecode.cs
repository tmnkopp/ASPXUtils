using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

namespace ASPXUtils
{ 

    public class EncodeDecode
    { 
        public static string Password = ConfigurationSettings.AppSettings["SITE_ID"];
 
        public static string Encode(string clearText)
        {
            return EncodeDecode.Encrypt(   clearText  , EncodeDecode.Password);
        }
        public static string Decode(string clearText)
        {
            return  EncodeDecode.Decrypt( clearText  , EncodeDecode.Password);
        }
        public static string EncodeURLSafe(string clearText )
        {
            return HttpContext.Current.Server.UrlEncode(
                EncodeDecode.Encrypt(
                clearText
                , EncodeDecode.Password));
        }
        public static string DecodeURLSafe(string clearText )
        {
            return HttpContext.Current.Server.UrlDecode(
                EncodeDecode.Decrypt(
                clearText  
                , EncodeDecode.Password)
                );
        }


        public static string EncodeURLSafe(string clearText, string password)
        {
            return HttpContext.Current.Server.UrlEncode(
                EncodeDecode.Encrypt(
                clearText
                , password));
        }
        public static string DecodeURLSafe(string clearText, string password)
        {
            return HttpContext.Current.Server.UrlDecode(
                EncodeDecode.Decrypt(
                clearText
                , password)
                );
        }

        public static string GetQSTREncripted(string Name)
        {
            Name = EncodeDecode.EncodeURLSafe(Name).Replace("%", "~");
            string Value = HTTPUtils.QString(Name);
            if (Value == "")
                return "";
            return EncodeDecode.Decode(Value);
        }
        public static string SetQSTREncripted(string Name, string Value)
        {
            if (Value == "" || Name=="")
                return "";
            return string.Format("{0}={1}", EncodeDecode.EncodeURLSafe(Name).Replace("%", "~"), EncodeDecode.EncodeURLSafe(Value));
        }

        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();

            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms,
               alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        } 
        public static string Encrypt(string clearText, string Password)
        { 
            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(clearText);
 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
 
            byte[] encryptedData = Encrypt(clearBytes,
                     pdb.GetBytes(32), pdb.GetBytes(16)); 
            return Convert.ToBase64String(encryptedData);

        } 

        public static byte[] Encrypt(byte[] clearData, string Password)
        { 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 
            return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));

        } 
        public static void Encrypt(string fileIn,
                    string fileOut, string Password)
        {
 
            FileStream fsIn = new FileStream(fileIn,
                FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                FileMode.OpenOrCreate, FileAccess.Write);
 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            Rijndael alg = Rijndael.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);
 
            CryptoStream cs = new CryptoStream(fsOut,
                alg.CreateEncryptor(), CryptoStreamMode.Write);
 
            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // encrypt it 
                cs.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
 
            cs.Close();
            fsIn.Close();
        }

 
        public static byte[] Decrypt(byte[] cipherData,
                                    byte[] Key, byte[] IV)
        { 
            MemoryStream ms = new MemoryStream();
 
            Rijndael alg = Rijndael.Create();  
            alg.Key = Key;
            alg.IV = IV; 
            CryptoStream cs = new CryptoStream(ms,
                alg.CreateDecryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the decryption 
            cs.Write(cipherData, 0, cipherData.Length); 
            cs.Close();
 
            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        } 

        public static string Decrypt(string cipherText, string Password)
        { 
            byte[] cipherBytes = Convert.FromBase64String(cipherText); 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 
            0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 
            byte[] decryptedData = Decrypt(cipherBytes,
                pdb.GetBytes(32), pdb.GetBytes(16));
 
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        } 

        public static byte[] Decrypt(byte[] cipherData, string Password)
        { 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}); 
            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }
 
        public static void Decrypt(string fileIn,
                    string fileOut, string Password)
        {
 
            FileStream fsIn = new FileStream(fileIn,
                        FileMode.Open, FileAccess.Read);
            FileStream fsOut = new FileStream(fileOut,
                        FileMode.OpenOrCreate, FileAccess.Write);
 
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            Rijndael alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16); 
            CryptoStream cs = new CryptoStream(fsOut,
                alg.CreateDecryptor(), CryptoStreamMode.Write);
 
            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                // read a chunk of data from the input file 
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                // Decrypt it 
                cs.Write(buffer, 0, bytesRead);

            } while (bytesRead != 0);

            // close everything 
            cs.Close(); // this will also close the unrelying fsOut stream 
            fsIn.Close();
        }
    }
}
