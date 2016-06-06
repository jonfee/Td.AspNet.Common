using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Td.AspNet.Utils
{
    public class Cryptography
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string txt)
        {
            MD5 md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(txt))).Replace("-", "");
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string txt)
        {
            byte[] StrRes = Encoding.UTF8.GetBytes(txt);
            HashAlgorithm iSHA = SHA1.Create();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
        /// <summary>
        /// Hash 加密采用的算法
        /// </summary>
        public enum HashFormat
        {
            MD516,
            MD532,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        };

        /// <summary>
        /// 对称加密采用的算法
        /// </summary>
        public enum SymmetricFormat
        {
            DES,
            TripleDES
        };

        /// <summary>
        /// 对字符串进行 Hash 加密
        /// </summary>
        public static string Hash(string inputString, HashFormat hashFormat)
        {
            HashAlgorithm algorithm = null;

            switch (hashFormat)
            {
                case HashFormat.MD516:
                    algorithm = MD5.Create();
                    break;
                case HashFormat.MD532:
                    algorithm = MD5.Create();
                    break;
                case HashFormat.SHA1:
                    algorithm = SHA1.Create();
                    break;
                case HashFormat.SHA256:
                    algorithm = SHA256.Create();
                    break;
                case HashFormat.SHA384:
                    algorithm = SHA384.Create();
                    break;
                case HashFormat.SHA512:
                    algorithm = SHA512.Create();
                    break;
            }

            algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            if (hashFormat == HashFormat.MD516)
            {
                return BitConverter.ToString(algorithm.Hash).Replace("-", "").Substring(8, 16).ToUpper();
            }
            else
            {
                return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
            }
        }

        /// <summary>
        /// 对字符串进行对称加密
        /// </summary>
        public static string SymmetricEncrypt(string inputString, SymmetricFormat symmetricFormat, string key, string iv)
        {
            SymmetricAlgorithm algorithm = null;

            switch (symmetricFormat)
            {
                case SymmetricFormat.DES:
                    algorithm = DES.Create();
                    break;
                case SymmetricFormat.TripleDES:
                    algorithm = TripleDES.Create();
                    break;
            }

            int keySize = algorithm.Key.Length;

            byte[] desString = Encoding.UTF8.GetBytes(inputString);

            byte[] desKey = Encoding.UTF8.GetBytes(key.Substring(0, keySize));

            byte[] desIV = Encoding.UTF8.GetBytes(iv.Substring(0, keySize));

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, algorithm.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return BitConverter.ToString(mStream.ToArray()).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// 对字符串进行对称解密
        /// </summary>
        public static string SymmetricDecrypt(string inputString, SymmetricFormat symmetricFormat, string key, string iv)
        {
            SymmetricAlgorithm algorithm = null;

            switch (symmetricFormat)
            {
                case SymmetricFormat.DES:
                    algorithm = DES.Create();
                    break;
                case SymmetricFormat.TripleDES:
                    algorithm = TripleDES.Create();
                    break;
            }

            int keySize = algorithm.Key.Length;

            byte[] desString = new byte[inputString.Length / 2];

            for (int i = 0; i < inputString.Length; i += 2)
            {
                desString[i / 2] = byte.Parse(inputString.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            byte[] desKey = Encoding.UTF8.GetBytes(key.Substring(0, keySize));

            byte[] desIV = Encoding.UTF8.GetBytes(iv.Substring(0, keySize));

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, algorithm.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }

    }
}
