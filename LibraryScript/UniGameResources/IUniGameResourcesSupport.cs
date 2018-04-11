using System;
using System.Collections.Generic;
using System.Text;
class IUniGameResourcesSupport : UniGameResourcesSupport
{
    //当前的程序版本
    public override string ProgramVersion { get { return "1.000"; } }
    //如果填写为local,则表示使用本地资源包
    //这时候需要将所有的资源包都配置成本地的
    public override string DownloadUrlPath { get { return LoaclDownloadUrlPathMode; } }
    //public override string DownloadUrlPath { get { return "http://shengshigame.com/ottgame/bearsbike/AssetBundle/ver1.000/"; } }

    public override string XmlRsaPublicKey { get { return "<RSAKeyValue><Modulus>qJY5035tO2Nzgp7Tg8EkZI9qou2vn3HZpFxX7ScNaRxwUnFpkXF5ZHWJgPlm4VCW0DpCVmooYDx5ZCJsKANx01r5Lb9VFrqNJtUlfXNBlnz2zjDHIvbB1QPMB9a+k/TFbt5P+rtOuDbmswEUcDX+uMcANo/RkK5JiAsGHKx7Cz8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
    public override string XmlDesKey { get { return "B88DE37A-9A24-4E5A-98B8-47AE9C9D4D82"; } }
    
    public override string LuaRsaPublicKey { get { return "<RSAKeyValue><Modulus>qJY5035tO2Nzgp7Tg8EkZI9qou2vn3HZpFxX7ScNaRxwUnFpkXF5ZHWJgPlm4VCW0DpCVmooYDx5ZCJsKANx01r5Lb9VFrqNJtUlfXNBlnz2zjDHIvbB1QPMB9a+k/TFbt5P+rtOuDbmswEUcDX+uMcANo/RkK5JiAsGHKx7Cz8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
    public override string LuaDesKey { get { return "B88DE37A-9A24-4E5A-98B8-47AE9C9D4D82"; } }
}
