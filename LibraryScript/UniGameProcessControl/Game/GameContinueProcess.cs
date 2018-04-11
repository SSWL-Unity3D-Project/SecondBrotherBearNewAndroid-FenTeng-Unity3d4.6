using UnityEngine;
using System.Collections;

class GameContinueProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_Continue; } }
    public GameContinueProcess()
        : base()
    {

    }

    //主预制
    internal GameContinueControl m_GameContinueControl = null;

    //游戏结束时间
    internal float m_GameOverTime = 0.0f;


    //初始化函数
    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();
        
        
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);
        InitUI();
        m_GameOverTime = m_GameContinueControl.m_GameOverTime;
        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.Play("continue.ogg", true);
        timeLocker.IsLocked = true;
        ResetIsEnterStart();
        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
    }

    //回收付费状态
    public void ResetIsEnterStart()
    {
        ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = false;
        ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart = false;
    }

    //初始化界面
    public void InitUI()
    {
        m_GameContinueControl = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("GameContinueControl.prefab", GameRoot.gameResource).GetComponent<GameContinueControl>();
        m_GameContinueControl.Initialization();

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.gameObject.SetActive(false);

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.gameObject.SetActive(true);
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.Play();
    }

    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_Continue)
            {
                if (!((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart)
                {
                        //Debug.Log("继续游戏:::开始游戏1");
                    if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P1) && 
                                        InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1))
                        {
                           //Debug.Log("继续游戏:::已开始游戏1");
                            ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = true;
                            InputDevice.SetEggNums(0, UniGameOptionsDefine.CoinsEggNums_1P);
                            //((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                            ContinueGame();
                            StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                    }
                }
                if (!((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart)
                {
                        //Debug.Log("继续游戏:::开始游戏2");
                    if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P2) &&
                                InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2))
                        {
                            //Debug.Log("继续游戏:::已开始游戏2");
                            ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart = true;
                            InputDevice.SetEggNums(1, UniGameOptionsDefine.CoinsEggNums_1P);
                            //((RaceSceneControl)GameRoot.CurrentSceneControl).SetPlayerIsEnterStart();
                            ContinueGame();
                            StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                    }
                }
            }
        }
    }

    //继续游戏
    private void ContinueGame()
    {
        //Debug.Log("继续游戏:::GameRaceProcess");
        m_GameContinueControl.StopShowGameOver();
        UnityEngine.GameObject.Destroy(m_GameContinueControl.gameObject);

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.gameObject.SetActive(false);

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.gameObject.SetActive(false);

        ((RaceSceneControl)GameRoot.CurrentSceneControl).ContinueGame();
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameRaceProcess));
    }

    //结束游戏
    private void EngGame()
    {
        //GameRoot.gameProcessControl.ActivateProcess(typeof(GameOverProcess));

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameContinueProcess.gameObject.SetActive(false);

        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.Stop();
        //((RaceSceneControl)GameRoot.CurrentSceneControl).m_DownPrompt_GameRaceProcess.gameObject.SetActive(false);

        UnityEngine.GameObject.Destroy(m_GameContinueControl.gameObject);
        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameEnd();

    }

    private FTLibrary.Time.TimeLocker timeLocker = new FTLibrary.Time.TimeLocker(1000);

    public override void OnUpdate()
    {
        m_GameOverTime -= Time.deltaTime;
        EnterStart();

        if (!timeLocker.IsLocked)
        {
            SoundEffectPlayer.Play("timeclock.wav");
            timeLocker.IsLocked = true;
        }
        if (m_GameOverTime <= 0)
        {
            EngGame();
            m_GameContinueControl.StopShowGameOver();
        }
    }
}
