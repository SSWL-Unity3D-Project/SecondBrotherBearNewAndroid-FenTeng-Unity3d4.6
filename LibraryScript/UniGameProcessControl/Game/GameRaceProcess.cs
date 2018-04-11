using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


class GameRaceProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_GameRace; } }
    public GameRaceProcess()
        : base()
    {

    }

    //游戏时间
    private float m_GameMaxTime = 0.0f;
    //游戏开始倒计时的时间
    private int m_RemainderMin = 10;

    enum GameProcess
    {
        gaming,
        end
    }

    private GameProcess m_GameProcess = GameProcess.gaming;

    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();

        base.Initialization();
        //Debug.Log("开始游戏了");
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);

        m_GameMaxTime = ((RaceSceneControl)GameRoot.CurrentSceneControl).sceneDefineList[((RaceSceneControl)GameRoot.CurrentSceneControl).currentSelectSceneId].gameTime;
        //Debug.Log("游戏时间:::" + m_GameMaxTime);
        InitUI();

        m_GameProcess = GameProcess.gaming;

        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();

        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.Play("game.ogg", true);
    }

    public void InitUI()
    {
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.gameObject.SetActive(true);
        ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerScore();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.Initialization();

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.gameObject.SetActive(false);

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.gameObject.SetActive(true);
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.Play();
    }

    //按键开始游戏
    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_GameRace)
            {
                    if (!((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart)
                    {
                        //Debug.Log("选择用户:::开始游戏1");
                        if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P1) &&
                                    InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1))
                        {
                            //Debug.Log("选择用户:::已开始游戏1");
                            ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = true;
                            InputDevice.SetEggNums(0, UniGameOptionsDefine.CoinsEggNums_1P);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiongDa.IsAIController = false;
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiongDa.IsInputController = true;
                            StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                        }
                }
                    if (!((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart)
                    {
                        //Debug.Log("选择用户:::开始游戏2");
                        if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P2) &&
                                    InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2))
                        {
                            //Debug.Log("选择用户:::已开始游戏2");
                            ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart = true;
                            InputDevice.SetEggNums(1, UniGameOptionsDefine.CoinsEggNums_1P);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiong2.IsAIController = false;
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiong2.IsInputController = true;
                            StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                        }
                }
            }
        }
    }

    private FTLibrary.Time.TimeLocker timeLocker = new FTLibrary.Time.TimeLocker(1000);

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_GameProcess == GameProcess.gaming)
        {
            EnterStart();

            m_GameMaxTime -= Time.deltaTime;
            //Debug.Log("倒计时:::" + m_GameMaxTime);
            if (m_GameMaxTime <= m_RemainderMin)
            {

                if (!((RaceSceneControl)GameRoot.CurrentSceneControl).m_Remainder.gameObject.activeSelf)
                {
                    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_Remainder.gameObject.SetActive(true);
                    SoundEffectPlayer.Play("timeclock.wav");
                    
                }

                if (!timeLocker.IsLocked)
                {
                    SoundEffectPlayer.Play("timeclock.wav");
                    timeLocker.IsLocked = true;
                }

                ((RaceSceneControl)GameRoot.CurrentSceneControl).m_Remainder.Text = ((int)m_GameMaxTime).ToString();

                if (m_GameMaxTime <= 0)
                {
                    m_GameProcess = GameProcess.end;
                    //ContinueUI();

                    ((RaceSceneControl)GameRoot.CurrentSceneControl).GameOver();

                    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_Remainder.gameObject.SetActive(false);
                }
            }
        }
    }

    //游戏结束 跳转到Continue界面
    private void ContinueUI()
    {
        //Debug.Log("继续界面:::GameContinueProcess");
        ((RaceSceneControl)GameRoot.CurrentSceneControl).ContinueGameUI();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).GetPlayerScore();
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameContinueProcess));
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.gameObject.SetActive(false);
    }

    ////显示继续游戏界面
    //private bool IsContinueGame = false;
    ////标记游戏币是否够
    //public bool IsCoinEnough = false;
    ////记录当前币数
    //public int ContinueplayerCoins = 0;

    ////当前游戏时间
    //public float CurrentgameTime = 0.0f;
    ////当前金币数
    //private int CurrentCoin = 0;

    ////初始化函数
    //public override void Initialization()
    //{
    //    base.Initialization();
    //    //加载完成后进行其他初始化
    //    CurrentCoin = 0;
    //    CurrentgameTime = 0.0f;
    //    GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>().DelegateOnPlayEndEvent = InitStrat;
    //    //初始化一次游戏轨迹任务
    //    TrackCommand.InitializationExecuteList(TrackCommand.gameRaceTrackCommand, TrackCommand.gameRaceTrackCommandExecute);
    //    //激活为当前监听源
    //    MusicPlayer.activeMyListener = true;
    //    //开始音乐播放
    //    MusicPlayer.Stop(true);
    //    MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
    //    MusicPlayer.Play("game.ogg", ture);

    //    //挂代理
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.DelegateOnPlayEndEvent = IsGameOnContiue;

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.DelegateOnPlayEndEvent = SelectContinueNoSelect;

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.DelegateOnPlayEndEvent = InitOthers;

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.DelegateOnPlayEndEvent = GameOverPlayEnd;
    //    IsContinueGame = false;
    //    IsCoinEnough = false;

    //    ContinueplayerCoins = 0;
    //}

    //public void InitStrat()
    //{
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.gameObject.SetActive(true);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.Play();
    //}

    //public void InitOthers()
    //{
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameStart.gameObject.SetActive(false);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameScore.gameObject.SetActive(true);

    //    //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameTime.gameObject.SetActive(true);
    //}

    //public void EatCoinsPlay()
    //{
    //    CurrentCoin += 10;
    //    UICoinsAnimControl TmpCoins = UICoinsAnimControl.GetCoinFromBuffer();
    //    TmpCoins.AnimPlay("+10");
    //    if (CurrentCoin <= 9999)
    //    {
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameScore.Text = CurrentCoin.ToString();
    //    }
    //    else
    //    {
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameScore.Text = "9999";
    //    }
    //}

    ////释放函数
    //public override void Dispose()
    //{

    //}

    ////判断游戏是否结束
    //public void IsGamePlayOver()
    //{
    //    //停止摇摆
    //    InputDevice.ShakeOff();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameScore.gameObject.SetActive(false);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameScore.Text = "";
    //    //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameTime.gameObject.SetActive(false);
    //    //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameTime.Text = "";

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.gameObject.SetActive(true);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.Play();

    //    SoundEffectPlayer.Play("youwin.wav");
    //}

    ////是否在继续界面
    //public void IsGameOnContiue()
    //{
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameWin.gameObject.SetActive(false);

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.gameObject.SetActive(true);
    //    IsContinueGame = true;
    //    IsCoinEnough = ConsoleCenter.IsCarContinueGame;
    //    ContinueplayerCoins = ConsoleCenter.playerCoins;
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.Play();

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins.gameObject.SetActive(true);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_ShowInsertCoins.Play();

    //    //SoundEffectPlayer.Play("gamecontinueorgameover.wav");
    //    SoundEffectPlayer.Play("askgamecontinue.wav");
    //}

    ////void SelectContinue()
    ////{
    ////    if(InputDevice.ButtonFire)
    ////    {
    ////        if(ConsoleCenter.IsCanStartGame)
    ////        {
    ////            ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart();
    ////             PlayerControl.m_IsGameOver = false;
    ////        }
    ////    }
    ////}

    //void SelectContinueNoSelect()
    //{
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.gameObject.SetActive(false);

    //    if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
    //    {
    //        StandbyProcess.UpdateCoinsUI();
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameEnd();
    //        PlayerControl.m_IsGameOver = false;
    //        return;
    //    }

    //    if (ConsoleCenter.IsCarContinueGame)
    //    {
    //        IsContinueGame = false;
    //        ConsoleCenter.PlayerContinueGamePayCoins();
    //        StandbyProcess.UpdateCoinsUI();
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart();
    //        PlayerControl.m_IsGameOver = false;
    //        return;
    //    }



    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.gameObject.SetActive(true);
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.Play();
    //    SoundEffectPlayer.Play("gamecontinueorgameover.wav");
    //    SoundEffectPlayer.Play("askgameover.wav");
    //}

    //void GameOverPlayEnd()
    //{
    //    //停止当前所有动画并隐藏
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.gameObject.SetActive(false);

    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.Stop();
    //    ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.gameObject.SetActive(false);


    //    IsContinueGame = false;
    //    if (ConsoleCenter.IsCarContinueGame)
    //    {
    //        ConsoleCenter.PlayerContinueGamePayCoins();
    //        StandbyProcess.UpdateCoinsUI();
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart();
    //        PlayerControl.m_IsGameOver = false;
    //    }
    //    else
    //    {
    //        StandbyProcess.UpdateCoinsUI();
    //        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameEnd();
    //        PlayerControl.m_IsGameOver = false;
    //        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameContinue.gameObject.SetActive(false);
    //        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.gameObject.SetActive(true);
    //        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.Stop();
    //        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameOver.Play();
    //    }
    //}

    //public override void OnUpdate()
    //{
    //    base.OnUpdate();

    //    if (IsContinueGame == true)
    //    {
    //        //在继续游戏之前币已经足够
    //        if (IsCoinEnough == true)
    //        {
    //            if ((InputDevice.ButtonFire && ConsoleCenter.IsCarContinueGame) || (ContinueplayerCoins < ConsoleCenter.playerCoins && ConsoleCenter.IsCarContinueGame))
    //            {
    //                GameOverPlayEnd();
    //            }
    //        }
    //        else
    //        {
    //            if (ConsoleCenter.IsCarContinueGame)
    //            {
    //                GameOverPlayEnd();
    //            }
    //        }
    //    }
    //    CurrentgameTime += (Time.fixedDeltaTime);
    //    //((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameTime.Text = CurrentgameTime.ToString("f0");
    //}
}

