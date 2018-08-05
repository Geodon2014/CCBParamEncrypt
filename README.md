# 建行支付接口ccbParam参数加密算法的c#实现

名称：建行支付接口ccbParam参数加密算法的c#实现<br />
作者：Geodon<br />
时间：2018-08-05<br />
描述：<br />
该算法根据建行提供的java版的算法翻译而来，算法解决了c#和java的部分实现机制上的差异<br />
使用c#实现该算法因语言上的不同有以下几点机制的差异<br />
 1. DES算法的填充方式：Java的PKCS5对应于c#的PKCS7
 2. DES算法的IV向量：java默认向量为空，c#需要显式声明一个空的byte数组，长度为8
 3. Base64编码机制：建行原java包中采用了sun.misc.BASE64Encoder类实现Base64编码，该类遵循了RFC822规定，该规定要求每76个字符加一个回车换行符，而c#默认的Convert.ToBase64String(str)没有回车换行符，c#想要遵循RFC822规定，需要增加第二个参数Base64FormattingOptions.InsertLineBreaks
 4. UrlEncoder编码机制：java的url编码模式采用大写，而c#则采用小写，如/，java编码结果为%2F，c#编码结果则为%2f
 5. utf-16编码机制：<br />
   5.1：java的utf-16默认编码顺序为高位优先（Big-endian），而c#的默认为低位优先（Little-Endian），解决该问题的办法是c#也采用高位优先，对应的编码名称是unicodeFFFE<br />
   5.2：java的utf-16编码后的byte数组相对于c#的utf-16编码后的byte数组，前两位多了-2和-1，因此为了保证一致，需要c#在byte数组前插入-2和-1，但c#的byte值是无符号的，所以不支持负值，要想处理负值，则需要按照如下方式处理：(byte)(0xff & 负值)

### 有关测试
有关算法使用及单元测试结果请参考：CCBParamEncryptTests.MCipherEncryptorTests
