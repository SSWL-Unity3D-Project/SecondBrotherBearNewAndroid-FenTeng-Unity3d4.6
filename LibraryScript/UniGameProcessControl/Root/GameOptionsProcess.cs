using System;
using System.Collections.Generic;
using UnityEngine;
class GameOptionsProcess:UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_GameOptions; } }

    private GameObject currentControlPanel = null;
    public string CurrentControlPanelName
    {
        set
        {
            if (currentControlPanel != null)
                UnityEngine.Object.DestroyImmediate(currentControlPanel);
            currentControlPanel = GameRoot.gameResource.LoadResource_Prefabs(value);
        }
    }
    public GameOptionsProcess()
        : base()
    {

    }
    //初始化函数
    public override void Initialization()
    {
        base.Initialization();
        //加载游戏配置组件
        CurrentControlPanelName = "ControlPanelRoot.prefab";
    }
    //释放函数
    public override void Dispose()
    {
        if (currentControlPanel != null)
            UnityEngine.Object.DestroyImmediate(currentControlPanel);
    }
}
