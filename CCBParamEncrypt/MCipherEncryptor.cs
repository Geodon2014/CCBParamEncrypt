/*
 * 名称：建行支付接口ccbParam参数加密算法的c#实现
 * 作者：Geodon
 * 时间：2018-08-05
 * 描述：
 * 该算法根据建行提供的java版的算法翻译而来，算法解决了c#和java的部分实现机制上的差异
 * 使用c#实现该算法因语言上的不同有以下几点机制的差异
 * 1.DES算法的填充方式：Java的PKCS5对应于c#的PKCS7
 * 2.DES算法的IV向量：java默认向量为空，c#需要显式声明一个空的byte数组，长度为8
 * 3.Base64编码机制：建行原java包中采用了sun.misc.BASE64Encoder类实现Base64编码，该类遵循了RFC822规定，该规定要求每76个字符加一个回车换行符，而c#默认的Convert.ToBase64String(str)没有回车换行符，c#想要遵循RFC822规定，需要增加第二个参数Base64FormattingOptions.InsertLineBreaks
 * 4.UrlEncoder编码机制：java的url编码模式采用大写，而c#则采用小写，如/，java编码结果为%2F，c#编码结果则为%2f
 * 5.utf-16编码机制：
 *    5.1：java的utf-16默认编码顺序为高位优先（Big-endian），而c#的默认为低位优先（Little-Endian），解决该问题的办法是c#也采用高位优先，对应的编码名称是unicodeFFFE
 *    5.2：java的utf-16编码后的byte数组相对于c#的utf-16编码后的byte数组，前两位多了-2和-1，因此为了保证一致，需要c#在byte数组前插入-2和-1，但c#的byte值是无符号的，所以不支持负值，要想处理负值，则需要按照如下方式处理：(byte)(0xff & 负值)
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CCBParamEncrypt
{
    public class MCipherEncryptor
    {
        private string encryptKey = "9R@e8Y3#";

        public MCipherEncryptor(string key)
        {
            encryptKey = key.Substring(0, 8);
        }

        public string getEncryptKey()
        {
            return encryptKey;
        }

        public void setEncryptKey(string encryptKey)
        {
            this.encryptKey = encryptKey.Substring(0, 8);
        }

        public static byte[] wrapBytes(byte[] srcBytes, byte[] wrapKey)
        {
            //编码后c#需要在byte数组前两位插入-2和-1
            int num = srcBytes.Length + 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                switch (i)
                {
                    case 0:
                        array[i] = (byte)(0xff & -2);
                        break;
                    case 1:
                        array[i] = (byte)(0xff & -1);
                        break;
                    default:
                        array[i] = srcBytes[i - 2];
                        break;
                }
            }
            using (DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider())
            {
                dESCryptoServiceProvider.Key = wrapKey;
                dESCryptoServiceProvider.Mode = CipherMode.ECB;
                dESCryptoServiceProvider.Padding = PaddingMode.PKCS7; //对应于java的PKCS5
                dESCryptoServiceProvider.IV = new byte[8]; //显式声明一个空的byte数组，长度为8
                 MemoryStream memoryStream = new MemoryStream();
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                }
                byte[] result = memoryStream.ToArray();
                memoryStream.Close();
                return result;
            }
        }

        public static string EncodeBase64String(byte[] srcBytes)
        {
            //遵循RFC822规定，适配sun.misc.BASE64Encoder的编码方式
            return Convert.ToBase64String(srcBytes, Base64FormattingOptions.InsertLineBreaks);
        }

        public string getEncodeString(string srcString)
        {
            byte[] srcBytes = wrapBytes(Encoding.GetEncoding("ISO-8859-1").GetBytes(srcString), Encoding.GetEncoding("ISO-8859-1").GetBytes(encryptKey));
            string text = EncodeBase64String(srcBytes);
            string srcString2 = text.Replace("+", ",");
            return urlEncoderForJava(srcString2, Encoding.GetEncoding("ISO-8859-1"));
        }

        public string doEncrypt(string srcString)
        {
            //c#的unicodeFFFE表示高位优先（Big-endian）
            byte[] srcBytes = wrapBytes(Encoding.GetEncoding("unicodeFFFE").GetBytes(srcString), Encoding.GetEncoding("ISO-8859-1").GetBytes(encryptKey));
            string text = EncodeBase64String(srcBytes);
            string srcString2 = text.Replace("+", ",");
            return urlEncoderForJava(srcString2, Encoding.GetEncoding("ISO-8859-1"));
        }

        /// <summary>
        /// 重写UrlEncoder方法，编码结果采用大写
        /// </summary>
        /// <param name="srcString">待编码的字符串</param>
        /// <param name="encoding">字符编码</param>
        /// <returns></returns>
        public string urlEncoderForJava(string srcString, Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < srcString.Length; i++)
            {
                string text = srcString[i].ToString();
                string text2 = HttpUtility.UrlEncode(text, encoding);
                if (text == text2)
                {
                    stringBuilder.Append(text);
                }
                else
                {
                    stringBuilder.Append(text2.ToUpper());
                }
            }
            return stringBuilder.ToString();
        }
    }
}
