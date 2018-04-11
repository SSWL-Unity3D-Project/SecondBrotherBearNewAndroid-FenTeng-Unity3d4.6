using System;
using System.Collections.Generic;
using UnityEngine;


class GameOverProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_GameOver; } }
    public GameOverProcess()
        : base()
    {

    }

    //倒计时是否播完
    public bool IsContinuePlay = false;
    //按钮锁
    public bool IsLock = true;

    //待机游戏每局的时间
    private float m_InningGameStandByMaxTime = 0.0f;
    private bool m_Reset = false;

    //初始化函数
    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();
        
        base.Initialization();
//#if _IgnoreVerify
//#else
//        //进入待机画面做一次完全安全验证
//        //如果验证失败了游戏就会被挂起
//        //做校验
//        GameRoot.CheckCipherText(StandbyProcess.VerifyEnvironmentKey_LogoVideo);
//#endif//_IgnoreVerify
        //GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>().DelegateOnPlayEndEvent = InitUI;
        m_InningGameStandByMaxTime = ((RaceSceneControl)GameRoot.CurrentSceneControl).sceneDefineList[((RaceSceneControl)GameRoot.CurrentSceneControl).currentSelectSceneId].gameTime;
        
        m_Reset = false;
        InitUI();

        //激活为当前监听源
        MusicPlayer.activeMyListener = true;
        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Standby;
        MusicPlayer.Play("game.ogg", true);

        
    }

    //关闭
    public void InitUI()
    {
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_Remainder.gameObject.SetActive(false);

        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.gameObject.SetActive(true);
        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerScore();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.Initialization();

        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameComLogo.gameObject.SetActive(true);
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameComLogo.Stop();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameComLogo.Play();

        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);

        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameProcess = RaceSceneControl.GameProcessLock.GameOver;
        IsLock = false;
        //初始化一次待机轨迹任务
        //TrackCommand.InitializationExecuteList(TrackCommand.standbyTrackCommand, TrackCommand.standbyTrackCommandExecute);
        
    }

    //按键开始游戏
    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_GameOver)
            {
                    //Debug.Log("待机:::开始游戏1");
                if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P1) &&
                                InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1))
                    {
                        //Debug.Log("待机:::已开始游戏1");
                        //加载完成后进行其他初始化
                        //((RaceSceneControl)GameRoot.CurrentSceneControl).GameSelectPlayer();
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = true;
                        InputDevice.SetEggNums(0, UniGameOptionsDefine.CoinsEggNums_1P);
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                        GameSelectPlayer();
                        ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                }
                    //Debug.Log("待机:::开始游戏2");
                if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P2) &&
                                InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2))
                    {
                        //Debug.Log("待机:::已开始游戏2");
                        //加载完成后进行其他初始化
                        //((RaceSceneControl)GameRoot.CurrentSceneControl).GameSelectPlayer();
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart = true;
                        InputDevice.SetEggNums(1, UniGameOptionsDefine.CoinsEggNums_1P);
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                        GameSelectPlayer();
                        ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                }
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!m_Reset)
        {
            m_InningGameStandByMaxTime -= Time.deltaTime;

            if (m_InningGameStandByMaxTime <= 0)
            {
                m_Reset = true;
                //GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);
                ((RaceSceneControl)GameRoot.CurrentSceneControl).WaitResetStandBy();
                return;
            }
        }
        EnterStart();
    }

    //选择人物
    public void GameSelectPlayer()
    {
        //Debug.Log("选择人物:::GameSelectPlayer");
        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetGameIsStandBy();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).GetPlayerScore();
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameSelectPlayerProcess));

        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameSelectMenu();
        //Invoke("GameSelectMap", 5.0f);
    }
}
