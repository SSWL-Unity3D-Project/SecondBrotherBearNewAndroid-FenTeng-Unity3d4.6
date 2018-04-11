using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using UnityEngine;


//场景启动模式
enum SceneWorkMode
{
    SCENEWORKMODE_Debug = 0,   //调试版
    SCENEWORKMODE_Release = 1   //正式版
}

partial class ConsoleCenter
{
    private void SetSystemCommandXml()
    {
        try
        {
            XmlDocument doc = GameRoot.gameResource.LoadResource_PublicXmlFile("systemcommand.xml");
            if (doc == null)
                return;
            XmlNode root = doc.SelectSingleNode("systemcommand");
            SystemCommand.targetFrameRate = Convert.ToInt32(root.SelectSingleNode("limitfps").Attribute("fps"));

        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
    }
    public void Initialization()
    {
        //读取明文系统配置信息
        SetSystemCommandXml();
        //设置渲染帧数
        Application.targetFrameRate = SystemCommand.targetFrameRate;
        //设置后台加载级别正常
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        //如果不在编辑器内隐藏鼠标
        if (!Application.isEditor)
        {
            //隐藏鼠标
            Screen.showCursor = false;
        }
        //设置游戏品质
        if (UniGameOptionsDefine.gameQualityLevel != -1)
        {
            if (QualitySettings.GetQualityLevel() != UniGameOptionsDefine.gameQualityLevel)
            {
                QualitySettings.SetQualityLevel(UniGameOptionsDefine.gameQualityLevel);
            }
        }
    }

    //当前需要加载的比赛场景使用的工作模式
    static public SceneWorkMode CurrentSceneWorkMode = SceneWorkMode.SCENEWORKMODE_Debug;
}
