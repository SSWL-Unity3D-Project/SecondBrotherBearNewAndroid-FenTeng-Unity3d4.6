using System;
using System.IO;
class UniGameOptionsDefine
{
    
    //游戏基本配置
    public static UniGameOptionsFile gameOptionsFile = null;
    public static UniInsertCoinsOptionsFile insertCoinsOptionsFile = null;

    public static void LoadGameOptionsDefault(UniGameResources gameResources)
    {
        try
        {
            //加载默认设置
            UniGameOptionsFile.LoadGameOptionsDefaultInfo("GameOptions.xml", gameResources);
            UniInsertCoinsOptionsFile.LoadInsertCoinsOptionsDefaultInfo("InsertCoinsOptions.xml", gameResources);
            gameOptionsFile = new UniGameOptionsFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath, "GameOptions\\GameOptions.dat"),
                                gameResources);
            insertCoinsOptionsFile = new UniInsertCoinsOptionsFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath, "GameOptions\\InsertCoinsOptions.dat"),
                                gameResources);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }
    //加载游戏配置选项信息
    public static void LoadGameOptionsDefine()
    {
        try
        {
            gameOptionsFile.LoadOptions();
            insertCoinsOptionsFile.LoadOptions();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }
    public static void RemoveAllOptions()
    {
        gameOptionsFile.RemoveOptions();
        insertCoinsOptionsFile.RemoveOptions();
    }

    public static void SerializeRead(UniOptionsFileBase[] fileList, MemoryStream s)
    {
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        for (int i = 0; i < fileList.Length;i++ )
        {
            writer.Write(fileList[i].OptionsType);
            fileList[i].SerializeRead(writer);
        }
    }
    public static void SerializeWrite(MemoryStream s)
    {
        s.Seek(0, SeekOrigin.Begin);
        BinaryReader reader = new BinaryReader(s);
        try
        {
            do
            {
                uint type = reader.ReadUInt32();
                UniOptionsFileBase file;
                if (type == gameOptionsFile.OptionsType)
                    file = gameOptionsFile;
                else if (type == insertCoinsOptionsFile.OptionsType)
                    file = insertCoinsOptionsFile;
                else
                    throw new Exception("unknow UniOptionsFile type!");
                file.SerializeWrite(reader);
            } while (reader.BaseStream.Position < reader.BaseStream.Length);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }

    //游戏难度设定
    public static UniGameOptionsFile.GameDifficulty gameDifficulty
    {
        get
        {
            return gameOptionsFile.gameDifficulty;
        }
        set
        {
            gameOptionsFile.gameDifficulty = value;
        }
    }
    //游戏语言设定
    //语言定义
    //中文
    private const uint ProductLanguage_Chinese = 0x00000001;
    //英文
    private const uint ProductLanguage_English = 0x00000002;
    //通用语言定义，这样就可以在产品里自己调了
    //case 0x7119C3FF://ProductLanguage_Common CRC32HashCode
    private const uint ProductLanguage_Common = 0x7119C3FF;

    private static uint deviceLanguageId
    {
        get
        {
            return FTLibrary.EliteDevice.EliteDevice.GetDeviceLanguage();
            //return ProductLanguage_English;
        }
    }
    public static UniGameResources.LanguageDefine gameLanguage
    {
        get 
        {
            UniGameResources.LanguageDefine ret;
            switch (deviceLanguageId)
            {
                case ProductLanguage_Chinese:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("SimplifiedChinese"),
                                        out ret);
                    break;
                case ProductLanguage_English:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("English"),
                                        out ret);
                    break;
                case ProductLanguage_Common:
                    ret = gameOptionsFile.gameLanguage;
                    break;
                default:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("English"),
                                    out ret);
                    break;
            }
            return ret;
        }
        set
        {
          
            gameOptionsFile.gameLanguage = value;
        }
    }

    //获取公司标志ID
    public static uint CompanyId
    {
        get
        {
            return FTLibrary.EliteDevice.EliteDevice.GetDeviceIntValue(0);
        }
    }

    //游戏声音设定
    public static float gameVolume
    {
        get { return gameOptionsFile.gameVolume; }
        set
        {
            gameOptionsFile.gameVolume = value;
        }
    }
    //待机背景音乐音量
    public static float StandByMusicVolume
    {
        get { return gameOptionsFile.StandByMusicVolume; }
        set
        {
            gameOptionsFile.StandByMusicVolume = value;
        }
    }
    //游戏显示分辨率设定
    public static UniGameOptionsFile.GameResolution gameResolution
    {
        get { return gameOptionsFile.gameResolution; }
        set
        {
            gameOptionsFile.gameResolution = value;
        }
    }
    public static UnityEngine.Resolution gameResolutionUnity
    {
        get
        {
            return gameOptionsFile.gameResolutionUnity;
        }
    }
    //游戏输入设备设定
    public static UniInputMode.InputMode gameInputMode
    {
        get { return gameOptionsFile.gameInputMode; }
        set
        {
            gameOptionsFile.gameInputMode = value;
        }
    }
    //io端口号
    public static int IOComIndex
    {
        get { return gameOptionsFile.IOComIndex; ; }
        set
        {
            gameOptionsFile.IOComIndex = value;
        }
    }
    //游戏品质
    public static int gameQualityLevel
    {
        get { return gameOptionsFile.gameQualityLevel; ; }
        set
        {
            gameOptionsFile.gameQualityLevel = value;
        }
    }


    //游戏收费模式
    public static UniInsertCoinsOptionsFile.GameChargeMode chargeMode
    { get { return insertCoinsOptionsFile.chargeMode; } set { insertCoinsOptionsFile.chargeMode = value; insertCoinsOptionsFile.SaveOptions(); } }
    //开始游戏需要几币
    public static int StartGameCoins
    { get { return insertCoinsOptionsFile.StartGameCoins; } set { insertCoinsOptionsFile.StartGameCoins = value; insertCoinsOptionsFile.SaveOptions(); } }
    //继续游戏需要几币
    public static int ContinueGameCoins
    { get { return insertCoinsOptionsFile.ContinueGameCoins; } set { insertCoinsOptionsFile.ContinueGameCoins = value; insertCoinsOptionsFile.SaveOptions(); } }
    //每投入几币奖励1个币
    public static int InsertConinsAwardConins
    { get { return insertCoinsOptionsFile.InsertConinsAwardConins; } set { insertCoinsOptionsFile.InsertConinsAwardConins = value; insertCoinsOptionsFile.SaveOptions(); } }
    //局域网对战胜利奖励几币
    public static int LanWinAwardConins
    { get { return insertCoinsOptionsFile.LanWinAwardConins; } set { insertCoinsOptionsFile.LanWinAwardConins = value; insertCoinsOptionsFile.SaveOptions(); } }
    //全国胜利奖励几币
    public static int WanWinAwardConins
    { get { return insertCoinsOptionsFile.WanWinAwardConins; } set { insertCoinsOptionsFile.WanWinAwardConins = value; insertCoinsOptionsFile.SaveOptions(); } }
    //一币兑换几秒
    public static int CoinsToSecond
    { get { return insertCoinsOptionsFile.CoinsToSecond; } set { insertCoinsOptionsFile.CoinsToSecond = value; insertCoinsOptionsFile.SaveOptions(); } }


    //-----******-----//

    //出彩票模式
    public static UniGameOptionsFile.GiftMode GiftModeFunc
    {
        get { return gameOptionsFile.GiftModeFunc; ; }
        set
        {
            gameOptionsFile.GiftModeFunc = value;
        }
    }

    //1P投币出扭蛋模式
    public static UniGameOptionsFile.CoinsEggMode CoinsEggMode_1P
    {
        get { return gameOptionsFile.CoinsEggMode_1P; ; }
        set
        {
            gameOptionsFile.CoinsEggMode_1P = value;
        }
    }
    //1P投币出扭蛋数量
    public static int CoinsEggNums_1P
    {
        get { return gameOptionsFile.CoinsEggNums_1P; ; }
        set
        {
            gameOptionsFile.CoinsEggNums_1P = value;
        }
    }
    //2P投币出扭蛋模式
    public static UniGameOptionsFile.CoinsEggMode CoinsEggMode_2P
    {
        get { return gameOptionsFile.CoinsEggMode_2P; ; }
        set
        {
            gameOptionsFile.CoinsEggMode_2P = value;
        }
    }
    //2P投币出扭蛋数量
    public static int CoinsEggNums_2P
    {
        get { return gameOptionsFile.CoinsEggNums_2P; ; }
        set
        {
            gameOptionsFile.CoinsEggNums_2P = value;
        }
    }

    //每彩蛋对应分数
    public static int PerEggScore
    {
        get { return gameOptionsFile.PerEggScore; ; }
        set
        {
            gameOptionsFile.PerEggScore = value;
        }
    }

    //彩蛋机自停时间
    public static int AutoSleepTime
    {
        get { return gameOptionsFile.AutoSleepTime; ; }
        set
        {
            gameOptionsFile.AutoSleepTime = value;
        }
    }

    //每票对应分数
    public static int PerTicketScore
    {
        get { return gameOptionsFile.PerTicketScore; ; }
        set
        {
            gameOptionsFile.PerTicketScore = value;
        }
    }

    //自行车轮最大转速/分钟
    public static int MaxRotateSpeed 
    {
        get { return gameOptionsFile.MaxRotateSpeed; }
        set
        {
            gameOptionsFile.MaxRotateSpeed = value;
        }
    }

}
