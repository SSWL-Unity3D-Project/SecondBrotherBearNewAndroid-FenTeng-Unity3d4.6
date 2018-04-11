using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Time;
using UnityGUI;
using UnityEngine;
using System.Threading;
using System.IO;

class SelfCheckingModalProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_SelfChecking; } }
    public SelfCheckingModalProcess()
        : base()
    {

    }
    /// <summary>
    /// 字符串
    /// </summary>
    private string str = "";

    /// <summary>
    /// 最大的行数
    /// </summary>
    public static int MaxLineCount = 40;

    /// <summary>
    /// 当前的行数
    /// </summary>
    private int currentLineCount = 0;

    /// <summary>
    /// 界面group
    /// </summary>
    public GuiModule guiModule;

    /// <summary>
    /// 文字
    /// </summary>
    private GuiLabelTextDoc textdoc;

    //显示输出信息
    void ShowMessage(string msg)
    {
        if (str == "")
        {
            currentLineCount = 1;
            str = msg;
        }
        else
        {
            if (currentLineCount >= MaxLineCount)
            {
                int idx = str.IndexOf('\n');
                //去掉最顶上一行
                str = str.Substring(idx + 1);
            }
            else
            {
                currentLineCount++;
            }
            str += "\n" + msg;
        }

        textdoc.Text = str;
    }
    enum CheckedEvent
    {
        Eve_Noting,                 //没有开始
        Eve_Start,                 //开始自检
        Eve_ProduceActivateInfo,   //汇报产品激活情况
        Eve_ProduceVersion,        //汇报产品版本情况
        Eve_InputDeviceResponse,    //输入设备响应
        Eve_InputDeviceResponseOver,//输入设备响应完成
        Eve_InputDeviceOut,         //输入设备输出
        Eve_InputDeviceOutOver,     //完成
        Eve_GameResolution,         //调整游戏分辨率
        Eve_GameResolution1,         //调整游戏分辨率
        Eve_IntoControlPanel,       //控制界面
        Eve_IntoGame,                //进入游戏
        Eve_IntoGameOk
    }
    private CheckedEvent checkedEvent = CheckedEvent.Eve_Noting;
    //初始化函数
    public override void Initialization()
    {
        //首先在启动过程中先查看是否出现了错误，如果出现了错误需要显示错误信息
        //然后就不进行下面的过程了
        if (ErrMessageBox.IsShowErrMessageBox)
        {
            ErrMessageBox.ShowErrMessage("游戏启动发生错误,停止自检过程!");
            return;
        }
        //进入游戏待机画面
        base.Initialization();
        guiModule=GameRoot.gameResource.LoadResource_Prefabs("SelfCheckingWindows.prefab").GetComponent<GuiModule>();
        //创建出来自检画面
        guiModule.mainGroup.AutoWidth = new GuiVector2(1.0f, 0.0f);
        guiModule.mainGroup.AutoHeight = new GuiVector2(1.0f, 0.0f);
        guiModule.mainGroup.BackgroupSeting = new GuiBackgroup(guiModule.skin.box);
        textdoc = guiModule.RegisterGuiLableTextDoc("", "", 0.1f, 0.1f, guiModule.skin.label, Color.white, int.MaxValue);
        textdoc.AutoWidth = new GuiVector2(1.0f, 0.0f);
        textdoc.AutoHeight = new GuiVector2(1.0f, 0.0f);
        //再次加载游戏配置信息，因为从控制台退出后也是会重新开始这个过程的
        UniGameOptionsDefine.LoadGameOptionsDefine();
        //显示非致命错误
        if (GameRoot.non_fatal_error_list != null)
        {
            ShowMessage("发生非致命错误:");
            for (int i = 0; i < GameRoot.non_fatal_error_list.Count; i++)
            {
                ShowMessage(GameRoot.non_fatal_error_list[i]);
            }
            GameRoot.non_fatal_error_list = null;
            ShowMessage("");
            ShowMessage("");
        }
        checkedEvent = CheckedEvent.Eve_Start;
        SelfCheckingProcess(null);
    }
    //释放函数
    public override void Dispose()
    {
        if (guiModule != null)
        {
            GameObject.DestroyImmediate(guiModule.gameObject);
            guiModule = null;
        }
    }
    private const long EveDelayTime = 1000;
    private const string PartingLine = "-----------------------------------------------------";
    void SelfCheckingProcess(object[] parameters)
    {
        switch (checkedEvent)
        {
            case CheckedEvent.Eve_Start:
                {
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str1"));//开始系统自检...
                    checkedEvent = CheckedEvent.Eve_ProduceActivateInfo;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_ProduceActivateInfo:
                {
                    ShowMessage(PartingLine);
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str2"));//产品激活成功!
                    ShowMessage(string.Format(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str3"),
                                        GameRoot.produceActivateId.activateId,
                                        GameRoot.produceActivateId.activateDate));//产品编码:{0} 激活日期:{1}
                    checkedEvent = CheckedEvent.Eve_ProduceVersion;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_ProduceVersion:
                {
                    FTLibrary.Produce.ProduceVersionInformation.VersionInfo version = GameRoot.gameResource.produceVersion.mainVersion;
                    ShowMessage(string.Format(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str4"),
                                        version.version,
                                        version.compiledate,
                                        version.issuedate));//产品版本:{0} 生产日期:{1} 发布日期:{2}
                    checkedEvent = CheckedEvent.Eve_InputDeviceResponse;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_InputDeviceResponse:
                {
                    ShowMessage(PartingLine);
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str5"));//输入设备自检...
                    
                    checkedEvent = CheckedEvent.Eve_InputDeviceResponseOver;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                   
                }
                break;
            case CheckedEvent.Eve_InputDeviceResponseOver:
                {
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str6"));//输入设备自检完成
                    
                    checkedEvent = CheckedEvent.Eve_InputDeviceOut;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_InputDeviceOut:
                {
                    ShowMessage(PartingLine);
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str7"));//输出设备自检...

                    checkedEvent = CheckedEvent.Eve_InputDeviceOutOver;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_InputDeviceOutOver:
                {
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str8"));//输出设备自检完成!

                    checkedEvent = CheckedEvent.Eve_GameResolution;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_GameResolution:
                {
                    ShowMessage(PartingLine);
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str9"));//调整游戏最佳分辨率

                    if (UniGameOptionsDefine.gameResolution != UniGameOptionsFile.GameResolution.Resolution_Default)
                    {
                        UnityEngine.Resolution resolution = UniGameOptionsDefine.gameResolutionUnity;
                        Screen.SetResolution(resolution.width, resolution.height, true);
                    }
                    
                    //Resolution now = Screen.currentResolution;
                    //Debug.Log(string.Format("{0},{1},{2}", now.width, now.height, now.refreshRate));
                    //Resolution[] resolutions = Screen.GetResolution;
                    //for (int i = 0; i < resolutions.Length;i++ )
                    //{
                    //    Debug.Log(string.Format("{0},{1},{2}", resolutions[i].width, resolutions[i].height, resolutions[i].refreshRate));
                    //}

                    checkedEvent = CheckedEvent.Eve_GameResolution1;
                    TimerCall(SelfCheckingProcess, 3000, false);
                }
                break;
            case CheckedEvent.Eve_GameResolution1:
                {
                    Resolution current=Screen.currentResolution;
                    ShowMessage(string.Format(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str9_1"),
                                        current.width, current.height, current.refreshRate));
                    ShowMessage(string.Format(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str9_2"),
                                        Screen.width, Screen.height));
                    checkedEvent = CheckedEvent.Eve_IntoControlPanel;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_IntoControlPanel:
                {
                    ShowMessage(PartingLine);
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str10"));
                    checkedEvent = CheckedEvent.Eve_IntoGame;
                    TimerCall(SelfCheckingProcess, 10000, false);
                }
                break;
            case CheckedEvent.Eve_IntoGame:
                {
                    ShowMessage(GameRoot.gameResource.LoadLanguageResource_Text("Check_Str11"));
                    checkedEvent = CheckedEvent.Eve_IntoGameOk;
                    TimerCall(SelfCheckingProcess, EveDelayTime, false);
                }
                break;
            case CheckedEvent.Eve_IntoGameOk:
                {
                    //在这里预载入声音资源
                    MusicPlayer.LoadAllUnloadAudioClip();
                    //进入公司LOGO过程
                    processControl.ActivateProcess(typeof(CompanyLogoProcess));
                }
                break;
        }
    }
    //public override void OnLateUpdate()
    //{
    //    if (checkedEvent == CheckedEvent.Eve_IntoGame)
    //    {
    //        if (InputDevice.SystemButton)
    //        {
    //            checkedEvent = CheckedEvent.Eve_Noting;
    //            //进入控制台过程
    //            processControl.ActivateProcess(typeof(GameOptionsProcess));
    //            return;
    //        }
    //    }
    //    if (InputDevice.SystemButton)
    //    {
    //        Debug.Log("这么快就进控制台啦!");
    //        checkedEvent = CheckedEvent.Eve_Noting;
    //        //进入控制台过程
    //        processControl.ActivateProcess(typeof(GameOptionsProcess));
    //    }
    //}
}
