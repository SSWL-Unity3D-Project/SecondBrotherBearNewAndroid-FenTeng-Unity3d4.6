using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
class SelfCheckingModalProcessCommon : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_SelfChecking; } }
    public SelfCheckingModalProcessCommon()
        : base()
    {

    }
    //初始化函数
    public override void Initialization()
    {
        //首先在启动过程中先查看是否出现了错误，如果出现了错误需要显示错误信息
        //然后就不进行下面的过程了
        if (ErrMessageBox.IsShowErrMessageBox)
        {
            ErrMessageBox.ShowErrMessage("游戏启动发生错误,无法进入游戏!");
            return;
        }
        //进入游戏待机画面
        base.Initialization();

        checkedEvent = CheckedEvent.Eve_Start;
        SelfCheckingProcess(null);
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
    private void SelfCheckingProcess(object[] parameters)
    {
        switch (checkedEvent)
        {
            case CheckedEvent.Eve_Start:
                {
                    checkedEvent = CheckedEvent.Eve_GameResolution;
                    TimerCall(SelfCheckingProcess, 1, false);
                }
                break;
            case CheckedEvent.Eve_GameResolution:
                {
                    if (UniGameOptionsDefine.gameResolution != UniGameOptionsFile.GameResolution.Resolution_Default)
                    {
                        UnityEngine.Resolution resolution = UniGameOptionsDefine.gameResolutionUnity;
                        Screen.SetResolution(resolution.width, resolution.height, true);
                    }
                    checkedEvent = CheckedEvent.Eve_IntoGameOk;
                    TimerCall(SelfCheckingProcess, 1, false);
                }
                break;
            case CheckedEvent.Eve_IntoGameOk:
                {
                    //在这里预载入声音资源
                    MusicPlayer.LoadAllUnloadAudioClip();

                    //进入公司LOGO过程
                    ConsoleCenter.CurrentSceneWorkMode = SceneWorkMode.SCENEWORKMODE_Release;
                    processControl.ActivateProcess(typeof(StandbyProcess));
                }
                break;

        }
    }
}
