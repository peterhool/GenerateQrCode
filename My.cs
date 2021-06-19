using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GitHubDemoTest
{
    class MyRijndaelManaged
    {
        //加密方法

        public static string AesEncrypt(string rawInput, string key, string iv)
        {
            if (string.IsNullOrEmpty(rawInput))
            {
                return string.Empty;
            }

            byte[] bytekey = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] byteiv = System.Text.Encoding.UTF8.GetBytes(iv);

            if (bytekey == null || byteiv == null || bytekey.Length < 1 || byteiv.Length < 1)
            {
                throw new ArgumentException("Key/Iv is null.");
            }

            using (var rijndaelManaged = new RijndaelManaged()
            {
                Key = bytekey, // 密钥，长度可为128， 196，256比特位
                IV = byteiv,  //初始化向量(Initialization vector), 用于CBC模式初始化
                KeySize = 128,//接受的密钥长度
                BlockSize = 128,//加密时的块大小，应该与iv长度相同
                Mode = CipherMode.CBC,//加密模式
                Padding = PaddingMode.PKCS7
            }) //填白模式，对于AES, C# 框架中的 PKCS　＃７等同与Java框架中 PKCS #5
            {
                using (var transform = rijndaelManaged.CreateEncryptor(bytekey, byteiv))
                {
                    var inputBytes = System.Text.Encoding.UTF8.GetBytes(rawInput);//字节编码， 将有特等含义的字符串转化为字节流
                    var encryptedBytes = transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);//加密
                    return Convert.ToBase64String(encryptedBytes);//将加密后的字节流转化为字符串，以便网络传输与储存。
                }
            }
        }

        //解密方法
        public static string AesDecrypt(string encryptedInput, string key, string iv)
        {
            if (string.IsNullOrEmpty(encryptedInput))
            {
                return string.Empty;
            }

            byte[] bytekey = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] byteiv = System.Text.Encoding.UTF8.GetBytes(iv);

            if (bytekey == null || byteiv == null || bytekey.Length < 1 || byteiv.Length < 1)
            {
                throw new ArgumentException("Key/Iv is null.");
            }

            using (var rijndaelManaged = new RijndaelManaged()
            {
                Key = bytekey,
                IV = byteiv,
                KeySize = 128,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                using (var transform = rijndaelManaged.CreateDecryptor(bytekey, byteiv))
                {
                    var inputBytes = Convert.FromBase64String(encryptedInput);
                    var encryptedBytes = transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return System.Text.Encoding.UTF8.GetString(encryptedBytes);

                }
            }
        }

    }
}
