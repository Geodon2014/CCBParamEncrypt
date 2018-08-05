using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCBParamEncrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCBParamEncrypt.Tests
{
    [TestClass()]
    public class MCipherEncryptorTests
    {
        [TestMethod()]
        public void doEncryptTest()
        {
            string expected = "ks4vEQwIFdDyd2j%2FFbXzeKNTJP38ZobnVi8izgr8va%2F1tPougp0XzInRGBl8HYbQr55eJAl4%2C%2FcG%0D%0AA9DCHuedDwYD0MIe550PVCtlNQdBPCRZpM97HS2a0gYD0MIe550PBgPQwh7nnQ%2FRHxSzIbQMYFFi%0D%0ACQd4eSgrPKxDHPY7gni8scyWMT1a5Xt56vzUkmgfkKasmdCaJ6db00n8JoPsZ%2CwYUMSfPASvBgPQ%0D%0Awh7nnQ8m4w%2FBPxqgrz3%2F2ekjq4Az5fHfbn19%2CaEu8OjhjPmu1wYD0MIe550PBgPQwh7nnQ%2FAA%2C8M%0D%0A7jDsWJI%2CiN3ezySMa0ksR%2Cqi0pBY0i74cyhvAAB4cVC7e5Fk6195inZ7xjeSPojd3s8kjBaO0tV6%0D%0AdlpGf8kUwZgLvd2DjSOT6Gb9VrjkFo5meJb5slBxTqlKGQbgR7xxFFvPk0G8T0SodvkO";
            //待加密参数串
            string strSrcParas = "MERFLAG=1&MERCHANTID=105000000000000&POSID=000000000&TERMNO1=&TERMNO2=&BRANCHID=110000000&ORDERID=105000000000000123456&QRCODE=CCB9991234567&AMOUNT=0.01&TXCODE=PAY100";
            //商户密钥后30位
            string strKey = "Er@9f7DE&e%3Ou^%";

            //1.创建COM.CCB. MCipherEncryptor对象
            MCipherEncryptor ccbEncryptor =
                    new MCipherEncryptor(strKey);

            //2.执行加密
            String ccbParam = ccbEncryptor.doEncrypt(strSrcParas);

            Assert.AreEqual(expected, ccbParam);
        }
    }
}