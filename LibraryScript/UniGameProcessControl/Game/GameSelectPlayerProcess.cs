using UnityEngine;
using System.Collections;

class GameSelectPlayerProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_SelectPlayer; } }
    public GameSelectPlayerProcess()
        : base()
    {

    }

    //背景界面
    internal Transform m_Background = null;
    //熊大 选择界面
    internal SelectBearControl m_SelectXiongDaControl = null;
    //熊2选择界面
    internal SelectBearControl m_SelectXiong2Control = null;

    //当前动画的播放状态枚举
    internal enum AnimationPlayState
    {
        One,    //一个
        Two,    //两个
        OneEnd, //一个已经播放完毕
    }
    internal AnimationPlayState m_AnimationPlayState = AnimationPlayState.One;

    //初始化函数
    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();
        
        base.Initialization();
        //Debug.Log("选择角色 初始化");
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);

        InitUI();
        IniAnimationPlayState();
        ControlUI();

        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
        MusicPlayer.Play("select.ogg", true);
    }

    //初始化动画的播放状态的模式
    private void IniAnimationPlayState()
    {
        //Debug.Log("初始化动画的播放状态的模式");
        if (((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiongDa.IsEnterStart && ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiong2.IsEnterStart)
        {
            m_AnimationPlayState = AnimationPlayState.Two;
            //Debug.Log("初始化动画的播放状态的模式:::两个玩家");
        }
        else if (((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiongDa.IsEnterStart || ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiong2.IsEnterStart)
        {
            m_AnimationPlayState = AnimationPlayState.One;
            //Debug.Log("初始化动画的播放状态的模式:::一个玩家");
        }
    }

    //初始化界面
    public void InitUI()
    {
        //Debug.Log("角色界面初始化完毕");
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.gameObject.SetActive(false);
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_GameComLogo.gameObject.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            ((RaceSceneControl)GameRoot.CurrentSceneControl).m_WaitMeUI[i].SetActive(false);
        }

        m_Background = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelectPlayerBackground.prefab", GameRoot.gameResource).GetComponent<Transform>();
        m_SelectXiongDaControl = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelecetXiongDa.prefab", GameRoot.gameResource).GetComponent<SelectBearControl>();
        m_SelectXiongDaControl.Initialization();
        m_SelectXiong2Control = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelecetXiong2.prefab", GameRoot.gameResource).GetComponent<SelectBearControl>();
        m_SelectXiong2Control.Initialization();
    }

    //控制要显示的界面和动画
    public void ControlUI()
    {
        //Debug.Log("控制要显示的界面和动画初始化完毕");
        //如果没有显示过被选择的界面 才进入
        if (m_SelectXiongDaControl.m_UIState == SelectControl.UIState.Ini)
        {
            if (((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart)
            {
                //Debug.Log("熊大可以玩");
                m_SelectXiongDaControl.ControlUI(true);
                m_SelectXiongDaControl.m_SelectControl.m_BackgroundPlaneAnimationPlayer.DelegateOnPlayEndEvent += PlaneAnimationEnd;
                m_SelectXiongDaControl.m_SelectControl.m_BackgroundPlaneAnimationPlayer.Play();
            }
            else
            {
                m_SelectXiongDaControl.ControlUI(false);
            }
        }
        if (m_SelectXiong2Control.m_UIState == SelectControl.UIState.Ini)
        {
            if (((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart)
            {
                //Debug.Log("熊2可以玩");
                m_SelectXiong2Control.ControlUI(true);
                m_SelectXiong2Control.m_SelectControl.m_BackgroundPlaneAnimationPlayer.DelegateOnPlayEndEvent += PlaneAnimationEnd;
                m_SelectXiong2Control.m_SelectControl.m_BackgroundPlaneAnimationPlayer.Play();
            }
            else
            {
                m_SelectXiong2Control.ControlUI(false);
            }
        }
    }

    //动画播放完毕
    public void PlaneAnimationEnd()
    {
        switch (m_AnimationPlayState)
        {
            case AnimationPlayState.One:
                {
                    //Debug.Log("一个角色 播放完毕");
                    GameSelectMap();
                    break;
                }
            case AnimationPlayState.Two:
                {
                    //Debug.Log("两个角色中的一个播放完毕");
                    m_AnimationPlayState = AnimationPlayState.OneEnd;
                    break;
                }
            case AnimationPlayState.OneEnd:
                {
                    //Debug.Log("两个角色的动画都播放完毕");
                    GameSelectMap();
                    break;
                }
        }
    }

    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_SelectPlayer)
            {
                    if (!((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart)
                    {
                        // Debug.Log("选择用户:::开始游戏1");
                        if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P1) &&
                             InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1))
                        {
                            // Debug.Log("选择用户:::已开始游戏1");
                            ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                            ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = true;
                            InputDevice.SetEggNums(0, UniGameOptionsDefine.CoinsEggNums_1P);
                            m_AnimationPlayState = AnimationPlayState.Two;
                            ControlUI();
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
                            m_AnimationPlayState = AnimationPlayState.Two;
                            ControlUI();
                            StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                        }
                }
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        EnterStart();
    }

    //卸载场景对象
    public override void Dispose()
    {
        base.Dispose();

        //Debug.Log("卸载选择用户界面");
        UnityEngine.GameObject.Destroy(m_Background.gameObject);
        m_Background = null;
        UnityEngine.GameObject.Destroy(m_SelectXiongDaControl.gameObject);
        m_SelectXiongDaControl = null;
        UnityEngine.GameObject.Destroy(m_SelectXiong2Control.gameObject);
        m_SelectXiong2Control = null;
    }

    //选择地图
    public void GameSelectMap()
    {
        //Debug.Log("选择地图:::GameSelectMap");
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameSelectMapProcess));

        //Invoke("GameStart", 5.0f);
    }
}
