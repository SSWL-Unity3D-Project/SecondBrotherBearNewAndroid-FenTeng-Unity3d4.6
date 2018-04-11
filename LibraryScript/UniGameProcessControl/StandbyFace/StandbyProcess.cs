using System;
using System.Collections.Generic;
using UnityEngine;


class StandbyProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_Standby; } }
    public StandbyProcess()
        : base()
    {

    }
    //投币数显示
    public static string[] CoinsNum = {"0","0"};
    //初始化函数
    public override void Initialization()
    {
        base.Initialization();
//#if _IgnoreVerify
//#else
        //进入待机画面做一次完全安全验证
        //如果验证失败了游戏就会被挂起
        GameRoot.CompleteVerify_Current_Environment();
        ////申请一次验证KEY
        //VerifyEnvironmentKey_LogoVideo = GameRoot.AllocVerifyEnvironmentKey(Guid.NewGuid().ToString());
//#endif//_IgnoreVerify


        //为了防止加载场景和IO卡线程冲突，这里暂时锁定，不释放资源
        UniGameResources.LockUnloadUnusedAssets();

        //加载游戏场景
        //如果是调试模式就不需要加载场景了
        if (ConsoleCenter.CurrentSceneWorkMode != SceneWorkMode.SCENEWORKMODE_Debug)
        {
            Application.LoadLevel(SystemCommand.FirstSceneName);
        }
        //加载完成后进行其他初始化
        //投币代理
        InputDevice.delegateInsertNewCoins = playerInsertCoins;

    }
    public static void playerInsertCoins(int coin, IParkourPlayer_Xiong.PlayerIndex index)
    {
        ConsoleCenter.OnPlayerInsertCoins(coin, index);
        UpdateCoinsUI(index);

        //if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_GameOver)
        //{
        //    if (ConsoleCenter.IsCanStartGame())
        //    {
        //        //加载完成后进行其他初始化
        //        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart();
        //        ConsoleCenter.PlayerStartGamePayCoins();
        //        UpdateCoinsUI();
        //    }
        //}
    }

    ////刷新币数
    public static void UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex index)
    {
        //for (int i = 0; i < ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins.Length; i++)
        //{
            //((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].gameObject.SetActive(false);
            //((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].gameObject.SetActive(false);




        int coins = ConsoleCenter.GetPlayerCoins(index);
        if (coins > 99)
        {
            coins = 99;
        }
        CoinsNum[(int)index] = coins.ToString();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameCoins[(int)index].Text = CoinsNum[(int)index];
        //如果已经进入游戏了，就什么都可以不用显示了
        if (((RaceSceneControl)GameRoot.CurrentSceneControl).IsEnterStart(index))
        {
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].gameObject.SetActive(false);
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].gameObject.SetActive(false);
        }
        //如果不够开始游戏就提示投币
        else if (!ConsoleCenter.IsCanStartGame(index))
        {
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].gameObject.SetActive(false);
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].gameObject.SetActive(true);
            if (((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].playStatus != GuiPlaneAnimationPlayer.PlayStatus.Status_Play)
            {
                ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].Stop();
                ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].Play();
            }
                
        }
        else
        {
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins[(int)index].gameObject.SetActive(false);
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].gameObject.SetActive(true);
            if (((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].playStatus != GuiPlaneAnimationPlayer.PlayStatus.Status_Play)
            {
                ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].Stop();
                ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowEnterJionGame[(int)index].Play();
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
        //{
        //    if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_GameOver)
        //    {
        //        if (InputDevice.ButtonLeft)
        //        {
        //            Debug.Log("开始游戏");
        //            if (ConsoleCenter.IsCanStartGame())
        //            {
        //                Debug.Log("可以开始游戏");
        //                //加载完成后进行其他初始化
        //                ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart();
        //                ConsoleCenter.PlayerStartGamePayCoins();
        //                UpdateCoinsUI();
        //            }
        //        }
        //    }
        //}
    }
//#if _IgnoreVerify
//#else
//    //释放函数
//    public override void Dispose()
//    {
//        GameRoot.RelaseVerifyEnvironmentKey(VerifyEnvironmentKey_LogoVideo);
//    }
//#endif //_IgnoreVerify
}
