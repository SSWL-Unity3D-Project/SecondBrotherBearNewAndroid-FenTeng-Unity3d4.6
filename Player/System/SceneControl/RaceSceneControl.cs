using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
class RaceSceneControl : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Race; } }

    //游戏预置对象
    public GameObject IParkourControllerPrefabs;
    //当前游戏控制
    public IParkourControllerScriptCS iParkourControllerScriptCS { get; set; }
    [System.Serializable]
    public class IParkourSceneDefine
    {
        public int sceneId;
        //开始等待时间
        public float startDelayCameraTime;
        //显示道具的等待时间
        public float startShowPropDelayTime;
        //游戏时间
        public float gameTime;
        //结束等待游戏时间
        public float endDelayCameraTime;
        //关联的场景对象，加载后替换预置内的
        public bool IsRandomPath;
        public GameObject[] patchesPrefabs;//patches that will be generated
        //关联的自行车轮粒子预置，加载后替换角色内的
        public GameObject frontWheelParticlePrefabs;
        public GameObject backWheelParticlePrefabs;

        //环视摄像机
        internal PlayerController startAroundCamera;
        internal PlayerController endAroundCamera;

        //雾的设置
        public bool fog;
        public Color fogColor;
        public float fogDensity;
    }

    public IParkourSceneDefine[] sceneDefineList = null;
    //当前选择的场景ID
    public int currentSelectSceneId { get; set; }
    //当前选择的场景定义对象
    public IParkourSceneDefine currentSelectSceneDefine { set; get; }

    //待机LOGO
    internal GuiPlaneAnimationPlayer m_GameComLogo = null;
    //待机的时候提示请投币
    internal GuiPlaneAnimationPlayer[] m_ShowInsertCoins = null;
    //有币 按键进入游戏的提示
    internal GuiPlaneAnimationPlayer[] m_ShowEnterJionGame = null;
    //玩家ID提示
    //internal PlayerIdBearPrefabControl[] m_PlayerIdBearPrefabControl = new PlayerIdBearPrefabControl[2];
    //投币数
    internal GuiPlaneAnimationText[] m_GameCoins = null;
    //游戏需要币数
    internal GuiPlaneAnimationText[] m_GameCoinsDeno = null;

    //得分提示
    internal PlayerScoreControl m_PlayerScoreControl = null;

    //游戏倒计时
    internal GuiPlaneAnimationTextAdvanced m_Remainder = null;

    //游戏开始摄像机环视等待时间
    public float m_StartGameWaiteCameraLookAround = 0.0f;

    //记录地图索引 在继续游戏的时候 使用
    private int m_SceneDefineListIndex = 0;
    /// <summary>
    /// "等等我"UI.
    /// </summary>
    internal GameObject[] m_WaitMeUI = null;

    //道具模版工厂
    public SenceObjectModControl m_SenceObjectModControl;

    //游戏开始
    //游戏胜利
    //游戏结束

    //游戏锁
    public enum GameProcessLock
    {
        GameNull,
        GameSelect, //选择界面中
        Gameing,    //游戏中
        GameOver,   //游戏结束
        GamingToGameOver,
        GameOverToGaming,
    }

    public GameProcessLock m_GameProcess = GameProcessLock.GameNull;

    //熊角色组
    IParkourPlayer_Xiong.PlayerIndex[] players = new IParkourPlayer_Xiong.PlayerIndex[2];

    //两个角色是否已付费开始
    //使用这个变量 是因为在切换界面 卸载所有内容的时候 需要保存角色上的付费状态 在开始游戏时将付费状态归还到角色身上
    //这个状态与角色付费状态同步 提供给流程使用
    private bool IsXiongDaIsEnterStart = false;
    public bool XiongDaIsEnterStart 
    { 
        get
        {
            return IsXiongDaIsEnterStart;
        }
        set
        {
            IsXiongDaIsEnterStart = value;
            InputDevice.SetFireLight((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1, IsXiongDaIsEnterStart);
        }
    }

    private bool IsXiong2IsEnterStart = false;
    public bool Xiong2IsEnterStart 
    {
        get
        {
            return IsXiong2IsEnterStart;
        }
        set
        {
            IsXiong2IsEnterStart = value;
            InputDevice.SetFireLight((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2, IsXiong2IsEnterStart);
        }
    }

    //获取指定角色是否进入了游戏
    public bool IsEnterStart(IParkourPlayer_Xiong.PlayerIndex playerindex)
    {
        switch (playerindex)
        {
            case IParkourPlayer_Xiong.PlayerIndex.Index_P1:
                return XiongDaIsEnterStart;
            case IParkourPlayer_Xiong.PlayerIndex.Index_P2:
                return Xiong2IsEnterStart;
        }
        return false;
    }


    //角色分数 //提供给流程使用
    public int XiongDaScore { get; set; }
    public int Xiong2Score { get; set; }

    protected override void Start()
    {
        base.Start();
    }
    protected override void LinkSceneInfo()
    {

    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        m_SenceObjectModControl.Initialization();

        //这里暂时直接启动为开始游戏
        //填充表示两个玩家都投币可以进行游戏
        //待机过程，填写为null
        //IParkourPlayer_Xiong.PlayerIndex[] players = { IParkourPlayer_Xiong.PlayerIndex.Index_P1, IParkourPlayer_Xiong.PlayerIndex.Index_P2 };
        //CreateIParkourScene(IParkourControllerScriptCS.GameStatus.status_NewGame, players);
        players[0] = IParkourPlayer_Xiong.PlayerIndex.Index_P1;
        players[1] = IParkourPlayer_Xiong.PlayerIndex.Index_P2;
        currentSelectSceneId = 0;

        int rp = UnityEngine.Random.Range(0, sceneDefineList.Length);
        CreateIParkourScene(IParkourControllerScriptCS.GameStatus.status_Standby, null, rp);

        SettingFog();

        GetPlayerIsEnterStart();

        InitUI();

        //如果是调试模式需要启动待机过程
        if (ConsoleCenter.CurrentSceneWorkMode == SceneWorkMode.SCENEWORKMODE_Debug)
        {
            GameRoot.gameProcessControl.ActivateAloneProcess(typeof(StandbyProcess));
            GameRoot.gameProcessControl.ActivateProcess(typeof(GameOverProcess));
        }
        else if (ConsoleCenter.CurrentSceneWorkMode == SceneWorkMode.SCENEWORKMODE_Release)
        {
            GameRoot.gameProcessControl.ActivateProcess(typeof(GameOverProcess));
        }

        Invoke("SetGameIsStart", 0.01f);

        //最后都初始化完成后取消启动画面
        if (SceneControl.gameBootFace != null)
        {
            SceneControl.gameBootFace.CloseGameBootFace();
            SceneControl.gameBootFace = null;
        }
    }

    //设置雾
    private void SettingFog()
    {

        RenderSettings.fog = currentSelectSceneDefine.fog;
        RenderSettings.fogColor = currentSelectSceneDefine.fogColor;
        RenderSettings.fogDensity = currentSelectSceneDefine.fogDensity;
    }

    //启动待机
    private void StartStandBy()
    {
        
        int rp = UnityEngine.Random.Range(0, sceneDefineList.Length);
        CreateIParkourScene(IParkourControllerScriptCS.GameStatus.status_Standby, null, rp);

        Invoke("SetGameIsStart", 0.01f);
    }

    //读取角色付费状态
    private void GetPlayerIsEnterStart()
    {
        XiongDaIsEnterStart = iParkourControllerScriptCS.playerXiongDa.IsEnterStart;
        Xiong2IsEnterStart = iParkourControllerScriptCS.playerXiong2.IsEnterStart;
    }

    //同步角色付费状态
    public void SetPlayerIsEnterStart()
    {
        iParkourControllerScriptCS.playerXiongDa.IsEnterStart = XiongDaIsEnterStart;
        iParkourControllerScriptCS.playerXiong2.IsEnterStart = Xiong2IsEnterStart;
    }

    //读取角色分数 只要是结束游戏 就进行一次读值 在流程中 界面初始化分数处进行
    public void GetPlayerScore()
    {
        XiongDaScore = iParkourControllerScriptCS.playerXiongDa.playerScore;
        Xiong2Score = iParkourControllerScriptCS.playerXiong2.playerScore;
    }

    //重置分数
    public void ResetPlayerScore()
    {
        XiongDaScore = 0;
        Xiong2Score = 0;
    }

    //同步角色分数 只要是开始游戏 就进行一次同步 在流程中 界面初始化分数处进行
    public void SetPlayerScore()
    {
        ResetPlayerScore();
        iParkourControllerScriptCS.playerXiongDa.playerScore = XiongDaScore;
        iParkourControllerScriptCS.playerXiong2.playerScore = Xiong2Score;
    }

    //初始化界面UI
    public void InitUI()
    {
        m_ShowInsertCoins = new GuiPlaneAnimationPlayer[2];
        m_ShowEnterJionGame = new GuiPlaneAnimationPlayer[2];
        m_GameCoins = new GuiPlaneAnimationText[2];
        m_GameCoinsDeno = new GuiPlaneAnimationText[2];
        m_WaitMeUI = new GameObject[2];

        m_GameComLogo = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("LOGO.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();

        m_ShowInsertCoins[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_InsertCoins_Left.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();
        m_ShowInsertCoins[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_InsertCoins_Right.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();

        m_ShowEnterJionGame[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_EnterJionGame_Left.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();
        m_ShowEnterJionGame[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_EnterJionGame_Right.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();

        m_GameCoins[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("MainUI_Coins_Left.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationText>();
        m_GameCoins[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("MainUI_Coins_Right.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationText>();
        m_GameCoinsDeno[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("MainUI_Coins_Denominator_Left.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationText>();
        m_GameCoinsDeno[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("MainUI_Coins_Denominator_Right.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationText>();
        //m_PlayerIdBearPrefabControl[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("PlayerIdBearPrefab_XiongDa.prefab", GameRoot.gameResource).GetComponent<PlayerIdBearPrefabControl>();
        //m_PlayerIdBearPrefabControl[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("PlayerIdBearPrefab_Xiong2.prefab", GameRoot.gameResource).GetComponent<PlayerIdBearPrefabControl>();

        m_PlayerScoreControl = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("PlayerScoreControl.prefab", GameRoot.gameResource).GetComponent<PlayerScoreControl>();
        m_PlayerScoreControl.Initialization();

        m_Remainder = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("MainUI_Remainder.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationTextAdvanced>();
        m_Remainder.gameObject.SetActive(false);

        m_WaitMeUI[0] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("WaitMe_Left.prefab", GameRoot.gameResource);
        m_WaitMeUI[1] = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("WaitMe_Right.prefab", GameRoot.gameResource);
        for (int i = 0; i < m_WaitMeUI.Length; i++)
        {
            m_WaitMeUI[i].SetActive(false);
        }

        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
        m_GameCoins[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P1].Text = StandbyProcess.CoinsNum[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P1];
        m_GameCoins[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P2].Text = StandbyProcess.CoinsNum[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P2];

        m_GameCoinsDeno[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P1].Text = UniGameOptionsDefine.StartGameCoins.ToString();
        m_GameCoinsDeno[(int)IParkourPlayer_Xiong.PlayerIndex.Index_P2].Text = UniGameOptionsDefine.StartGameCoins.ToString();
        //m_DownPrompt_GameOverProcess.Play();
        //m_DownPrompt_GameRaceProcess.gameObject.SetActive(false);
        //m_DownPrompt_GameContinueProcess.gameObject.SetActive(false);
    }

    protected override void OnDestroyScene()
    {
        base.OnDestroyScene();
    }
    //构造游戏场景，再调用这个函数之前保证已经给currentSelectSceneId赋值了，否则会加载默认场景
    public bool CreateIParkourScene(IParkourControllerScriptCS.GameStatus status, IParkourPlayer_Xiong.PlayerIndex[] players, int pathindex)
    {
        if (sceneDefineList == null || sceneDefineList.Length == 0)
            return false;
        for (int i = 0; i < sceneDefineList.Length; i++)
        {
            if (sceneDefineList[i].sceneId == currentSelectSceneId)
            {
                currentSelectSceneDefine = sceneDefineList[i];
                break;
            }
        }
        //Debug.Log("选择地图 编号:::" + pathindex);
        //if (currentSelectSceneDefine == null)
        currentSelectSceneDefine = sceneDefineList[pathindex];

        GameObject sceneObject = (GameObject)Instantiate((GameObject)IParkourControllerPrefabs);
        iParkourControllerScriptCS = sceneObject.GetComponent<IParkourControllerScriptCS>();
        iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, status, players);
        return true;
    }
    //卸载场景
    public void DestroyIParkourScene()
    {
        if (iParkourControllerScriptCS != null)
        {
            UnityEngine.GameObject.Destroy(iParkourControllerScriptCS.gameObject);
            iParkourControllerScriptCS = null;
        }
        currentSelectSceneId = 0;
        currentSelectSceneDefine = null;
    }
    //标记游戏已经开始了
    public void SetGameIsStart()
    {
        if (iParkourControllerScriptCS == null)
            return;
        iParkourControllerScriptCS.isGamePaused = false;
    }

    //游戏暂停
    public void SetGameIsStandBy()
    {
        if (iParkourControllerScriptCS == null)
            return;
        iParkourControllerScriptCS.isGamePaused = true;
    }

    public void WaitResetStandBy()
    {
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);
        Invoke("ResetStandBy",0.1f);
    }

    //重置待机
    public void ResetStandBy()
    { 
        GameSelectMenu();
        StartStandBy();
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameOverProcess)); 
    }

    //角色开始
    public void StartPlayerByIndex(IParkourPlayerController.PlayerType index)
    {
        switch (index)
        {
            case IParkourPlayerController.PlayerType.Type_XiongDa:
                ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiongDa.IsEnterStart = true;
                break;

            case IParkourPlayerController.PlayerType.Type_Xiong2:
                ((RaceSceneControl)GameRoot.CurrentSceneControl).iParkourControllerScriptCS.playerXiong2.IsEnterStart = true;
                break;
            default:

                break;
        }
    }

    //进入选择界面
    public void GameSelectMenu()
    {
        iParkourControllerScriptCS.hCheckPointsMainCS.hPatchesRandomizerCS.CleanPatch();
        DestroyIParkourScene();
    }

    //生成环绕相机
    public void IniAroundCamera(GameProcessLock gameprocess)
    {
        switch (gameprocess)
        {
            case GameProcessLock.Gameing:
                currentSelectSceneDefine.startAroundCamera = GameRoot.gameResource.LoadResource_Prefabs("CameraLookAroundStart.prefab").GetComponentInChildren<PlayerController>();
                currentSelectSceneDefine.startAroundCamera.StartLookAround();
                break;
            case GameProcessLock.GameOver:
                currentSelectSceneDefine.endAroundCamera = GameRoot.gameResource.LoadResource_Prefabs("CameraLookAroundEnd.prefab").GetComponentInChildren<PlayerController>();
                currentSelectSceneDefine.endAroundCamera.gameObject.transform.position = iParkourControllerScriptCS.gameObject.transform.position;
                currentSelectSceneDefine.endAroundCamera.StartLookAround();
                break;
        }
    }

    //改变道具显示状态
    public void ChangePropItemActive()
    {
        iParkourControllerScriptCS.hCheckPointsMainCS.hPatchesRandomizerCS.ChangePathPropItemActive();
    }

    //开始游戏
    public void GameStart(int PathIndex)
    {
        //Debug.Log("开始游戏");

        m_SceneDefineListIndex = PathIndex;

        CreateIParkourScene(IParkourControllerScriptCS.GameStatus.status_Standby, null, PathIndex);
        SettingFog();
        SetPlayerIsEnterStart();
        iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, IParkourControllerScriptCS.GameStatus.status_NewGame, players);
        
        IniAroundCamera(GameProcessLock.Gameing);

        //iParkourControllerScriptCS.m_LookAroundCamera.StartLookAround();

        //CreateIParkourScene(IParkourControllerScriptCS.GameStatus.status_NewGame, players);
        m_GameProcess = GameProcessLock.Gameing;
        //GameRoot.gameProcessControl.ActivateProcess(typeof(GameRaceProcess));
        //iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, IParkourControllerScriptCS.GameStatus.status_NewGame, players);
        Invoke("ChangePropItemActive", 0.3f);
        Invoke("ChangePropItemActive", currentSelectSceneDefine.startShowPropDelayTime);
        //当巡逻相机运行完成后才标记游戏开始
        //Invoke("SetGameIsStart", currentSelectSceneDefine.startDelayCameraTime);

        iParkourControllerScriptCS.playerXiong2.IsShowPrompt_Accelerate = true;
        iParkourControllerScriptCS.playerXiongDa.IsShowPrompt_Accelerate = true;

        //Invoke("GameEnd", sceneDefineList[0].gameTime);
        //Debug.Log("游戏结束倒计时开始");
    }

    //一场游戏结束 -- 一场游戏结束 可出现的效果
    public void GameOver()
    {
        //IniAroundCamera(GameProcessLock.GameOver);
        ChangePropItemActive();

        iParkourControllerScriptCS.playerCamera.camera.transform.position = iParkourControllerScriptCS.m_cameraGotoChange.transform.position;
        iParkourControllerScriptCS.playerCamera.camera.transform.eulerAngles = iParkourControllerScriptCS.m_cameraGotoChange.transform.eulerAngles;
        iParkourControllerScriptCS.playerXiong2.Release_Prompt_Accelerate();
        iParkourControllerScriptCS.playerXiongDa.Release_Prompt_Accelerate();
        iParkourControllerScriptCS.playerXiong2.IsShowPrompt_Accelerate = false;
        iParkourControllerScriptCS.playerXiongDa.IsShowPrompt_Accelerate = false;
        iParkourControllerScriptCS.playerGuangTouQiang.gameObject.SetActive(false);
        iParkourControllerScriptCS.hCheckPointsMainCS.hPatchesRandomizerCS.StopDestoryGoPreviousPatch();
        Invoke("ContinueGameUI", currentSelectSceneDefine.endDelayCameraTime);
    }

    //继续游戏选择界面
    public void ContinueGameUI()
    {
        iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, IParkourControllerScriptCS.GameStatus.status_Standby, players);
        GetPlayerIsEnterStart();
        //Debug.Log("玩家操作权限 收回:::" + iParkourControllerScriptCS.playerXiongDa.IsEnterStart + "|||" + iParkourControllerScriptCS.playerXiong2.IsEnterStart);
        SetGameIsStandBy();

        //清除所有内容 不然会出错
        GameSelectMenu();

        GameRoot.gameProcessControl.ActivateProcess(typeof(GameContinueProcess));
        m_PlayerScoreControl.gameObject.SetActive(false);
    }

    //继续游戏
    public void ContinueGame()
    {
        //iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, IParkourControllerScriptCS.GameStatus.status_ContinueGame, players);
        //Invoke("SetGameIsStart", 0.01f);
        GameStart(m_SceneDefineListIndex);
    }

    //游戏结束
    public void GameEnd()
    {
        //Debug.Log("游戏结束");
        m_GameProcess = GameProcessLock.GameOver;
        StartStandBy();
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameOverProcess));
        
        //iParkourControllerScriptCS.Initialization(currentSelectSceneDefine, IParkourControllerScriptCS.GameStatus.status_Standby, players);
        //Invoke("SetGameIsStart", 0.01f);
    }


    private GameObject ObjectTicketWarning_L = null;
    private GameObject ObjectTicketWarning_R = null;
    private void Update()
    {
        //刷新出票错误
        if (InputDevice.IsTicketWarning_Player[0])
        {
            if (ObjectTicketWarning_L == null)
            {
                ObjectTicketWarning_L = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("TicketWarning_L.prefab", GameRoot.gameResource);
            }
            
        }
        else
        {
            if (ObjectTicketWarning_L != null)
            {
                UnityEngine.Object.DestroyObject(ObjectTicketWarning_L);
                ObjectTicketWarning_L = null;
            }
        }

        if (InputDevice.IsTicketWarning_Player[1])
        {
            if (ObjectTicketWarning_R == null)
            {
                ObjectTicketWarning_R = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("TicketWarning_R.prefab", GameRoot.gameResource);
            }
        }
        else
        {
            if (ObjectTicketWarning_R != null)
            {
                UnityEngine.Object.DestroyObject(ObjectTicketWarning_R);
                ObjectTicketWarning_R = null;
            }
        }
    }
}
