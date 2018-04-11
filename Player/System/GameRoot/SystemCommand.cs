using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;

class SystemCommand
{
    public static void Initialization(XmlDocument doc)
    {
        XmlNode root = doc.SelectSingleNode("systemcommand");
        XmlNode node = root.SelectSingleNode("GameScene");
        FirstSceneName = node.Attribute("firstscenename");

        node = root.SelectSingleNode("CompanyLogoProcess");
        int companyId = (int)UniGameOptionsDefine.CompanyId;
        string chaeelVersion = "SS";
        XmlNodeList nodelist = node.SelectSingleNode("CompanyList").SelectNodes("Company");
        for (int i = 0; i < nodelist.Count; i++)
        {
            if (Convert.ToInt32(nodelist[i].Attribute("id")) == companyId)
            {
                chaeelVersion = nodelist[i].Attribute("name");
                break;
            }
        }
        node = node.SelectSingleNode(chaeelVersion);
        nodelist = node.SelectNodes("Step");
        CompanyLogoOrder = new string[nodelist.Count];
        for (int i = 0; i < CompanyLogoOrder.Length; i++)
        {
            CompanyLogoOrder[i] = nodelist[i].Attribute("texturename");
        }
        nodelist = node.SelectNodes("Logo");
        CompanyLogoPerfabOrder = new string[nodelist.Count];
        for (int i = 0; i < CompanyLogoPerfabOrder.Length; i++)
        {
            CompanyLogoPerfabOrder[i] = nodelist[i].Attribute("logoPrefabname");
        }
        //加载音乐配置
        MusicPlayer.Initialization();
        //加载音效配置
        SoundEffectPlayer.Initialization();
        //加载轨迹任务表
        //TrackCommand.LoadTrackCommandList(GameRoot.gameResource.LoadResource_XmlFile("TrackCommand.xml"));
    }
    //系统帧率限制
    public static int targetFrameRate = 60;
    //第一次进入的场景
    public static string FirstSceneName = "";
    //公司LOGO顺序
    public static string[] CompanyLogoOrder = null;
    //LOGO预制顺序
    public static string[] CompanyLogoPerfabOrder = null;
}
