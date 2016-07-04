using System;
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
        }

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

            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            if (hashFormat == HashFormat.MD516)
            {
                return BitConverter.ToString(bytes).Replace("-", "").Substring(8, 16).ToUpper();
            }
            else
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }

        }

    }
}
