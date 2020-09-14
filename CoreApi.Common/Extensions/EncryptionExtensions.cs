using System;
using System.Security.Cryptography;
using System.Text;

namespace CoreApi.Common.Extensions
{
    public static class EncryptionExtensions
    {
        private const string SecurityKey = "tQ&146eH@Za1t0sa@!q?";
        public static string Encrypt(this string toEncrypt, string securityKey = SecurityKey, bool useHashing = true)  
        {
            try
            {
                byte[] keyArray;  
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);  
   
                string key = securityKey;  
   
                //If hashing use get hashcode regards to your key  
                if (useHashing)  
                {  
                    var hashMd5 = new MD5CryptoServiceProvider();  
                    keyArray = hashMd5.ComputeHash(Encoding.UTF8.GetBytes(key));  
                    //Always release the resources and flush data of the Cryptographic service provide. Best Practice  
   
                    hashMd5.Clear();  
                }  
                else  
                    keyArray = Encoding.UTF8.GetBytes(key);

                var tDes = new AesCryptoServiceProvider
                {
                    //set the secret key for the tripleDES algorithm  
                    Key = keyArray, 
                    Mode = CipherMode.ECB, 
                    //padding mode(if any extra byte added)
                    Padding = PaddingMode.PKCS7
                };
            
                var cTransform = tDes.CreateEncryptor();  
                //transform the specified region of bytes array to resultArray  
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);  
                //Release resources held by TripleDes encryptor 
                tDes.Clear();  
                //Return the encrypted data into unreadable string format  
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
                return toEncrypt;
            } 
        }  
   
        public static string Decrypt(this string cipherString, string securityKey = SecurityKey, bool useHashing = true)  
        {
            try
            {
                byte[] keyArray;  
                //get the byte code of the string  
   
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);  
                string key = securityKey;  
   
                if (useHashing)  
                {  
                    //if hashing was used get the hash code with regards to your key  
                    var hashMd5 = new MD5CryptoServiceProvider();  
                    keyArray = hashMd5.ComputeHash(Encoding.UTF8.GetBytes(key));  
                    //release any resource held by the MD5CryptoServiceProvider
                    hashMd5.Clear();  
                }  
                else  
                {  
                    //if hashing was not implemented get the byte code of the key  
                    keyArray = Encoding.UTF8.GetBytes(key);  
                }  
   
                var tDes = new AesCryptoServiceProvider
                {
                    //set the secret key for the tripleDES algorithm  
                    Key = keyArray, 
                    Mode = CipherMode.ECB, 
                    //padding mode(if any extra byte added)
                    Padding = PaddingMode.PKCS7
                };
   
                var cTransform = tDes.CreateDecryptor();  
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);  
                //Release resources held by TripleDes Encryptor                  
                tDes.Clear();  
                //return the Clear decrypted TEXT  
                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return cipherString;
            }  
        } 
    }
}