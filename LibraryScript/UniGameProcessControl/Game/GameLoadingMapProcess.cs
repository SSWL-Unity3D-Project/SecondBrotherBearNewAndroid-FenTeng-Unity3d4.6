using UnityEngine;
using System.Collections;


class GameLoadingMapProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_LoadMap; } }
    public GameLoadingMapProcess()
        : base()
    {

    }

    //背景界面
    internal GameObject m_Background = null;
    //加载对象
    internal GameObject LoadObject = null;
    //进度条
    internal GuiPlaneAnimationProgressBar progressBar;
    //动画控制
    internal Animation loadAnimation;
    //动画左右位置
    internal Transform leftposition;
    internal Transform rightposition;
    //提示剩余的时间
    internal GuiPlaneAnimationTextAdvanced SelectMap_Remainder = null;
    //当前界面保留的时间
    internal const int RemainderTime = 20;
    internal float currentRemainderTime = 0.0f;
    internal int prveRemainderTime = 0;

    //进度控制
    //默认时间进度增加的进度量
    internal const float TimeProgressAdd =  1.0f/(float)RemainderTime;
    //玩家骑自行车后累计的进度量
    internal const float AccelerateProgressAdd = 0.2f;

    //当前进度
    internal float currentProgressValue = 0.0f;

    //初始化函数
    public override void Initialization()
    {
        base.Initialization();
        //Debug.Log("选择地图 初始化");
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);
        InitUI();
    }
    private void InitUI()
    {
        m_Background = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelectMapBackground.prefab", GameRoot.gameResource);
        LoadObject = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelecetMapLoadgame.prefab", GameRoot.gameResource);
        progressBar = GameObject.Find("/UICamera(Clone)/SelecetMapLoadgame(Clone)/bar").GetComponent<GuiPlaneAnimationProgressBar>();
        loadAnimation = GameObject.Find("/UICamera(Clone)/SelecetMapLoadgame(Clone)/X_PlayerRun").GetComponent<Animation>();
        leftposition = GameObject.Find("/UICamera(Clone)/SelecetMapLoadgame(Clone)/leftposition").GetComponent<Transform>();
        rightposition = GameObject.Find("/UICamera(Clone)/SelecetMapLoadgame(Clone)/rightposition").GetComponent<Transform>();
        //设置进度值
        progressBar.SetProgressBar(currentProgressValue);
        //设置动画
        loadAnimation.wrapMode = WrapMode.Loop;
        loadAnimation.CrossFade("X_PlayerRun");

        SelectMap_Remainder = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelectMap_Remainder.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationTextAdvanced>();
        currentRemainderTime = RemainderTime;
        UpdateTime();
        SoundEffectPlayer.Play("loadgame.wav");
    }
    private void UpdateTime()
    {
        if ((int)currentRemainderTime != prveRemainderTime)
        {
            prveRemainderTime = (int)currentRemainderTime;
            int value = prveRemainderTime + 1;
            SelectMap_Remainder.Text = value.ToString();
        }
    }
    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || 
            UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_LoadMap)
            {
                if (!((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart)
                {
                    //Debug.Log("选择地图:::开始游戏1");
                    if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P1) &&
                                InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1))
                    {
                        // Debug.Log("选择地图:::已开始游戏1");
                        ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaIsEnterStart = true;
                        InputDevice.SetEggNums(0, UniGameOptionsDefine.CoinsEggNums_1P);
                        StandbyProcess.UpdateCoinsUI(IParkourPlayer_Xiong.PlayerIndex.Index_P1);
                    }
                }
                if (!((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart)
                {
                    //Debug.Log("选择地图:::开始游戏2");
                    if (ConsoleCenter.IsCanStartGame(IParkourPlayer_Xiong.PlayerIndex.Index_P2) &&
                                InputDevice.ButtonEnter((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2))
                    {
                        //Debug.Log("选择地图:::已开始游戏2");
                        ConsoleCenter.PlayerStartGamePayCoins(IParkourPlayer_Xiong.PlayerIndex.Index_P2);
                        ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2IsEnterStart = true;
                        InputDevice.SetEggNums(1, UniGameOptionsDefine.CoinsEggNums_1P);
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
        if (LoadObject != null)
        {
            //计算本周期进度条增加量
            float accelerateValue = Mathf.Max(InputDevice.Accelerate((int)IParkourPlayer_Xiong.PlayerIndex.Index_P1),
                            InputDevice.Accelerate((int)IParkourPlayer_Xiong.PlayerIndex.Index_P2));
            if (accelerateValue > 0.0f)
            {
                loadAnimation.wrapMode = WrapMode.Loop;
                loadAnimation.CrossFade("X_PlayerSprint");
            }
            else
            {
                loadAnimation.wrapMode = WrapMode.Loop;
                loadAnimation.CrossFade("X_PlayerRun");
            }
            float processAddValue = TimeProgressAdd + Mathf.Lerp(0.0f, AccelerateProgressAdd, accelerateValue);
            //计算出来的是每秒的增加量，需要折算到当前周期
            currentProgressValue += Mathf.Lerp(0.0f, processAddValue, Time.deltaTime);
            progressBar.SetProgressBar(currentProgressValue);


            //更新坐标
            Vector3 p = loadAnimation.transform.localPosition;
            p.x = Mathf.Lerp(leftposition.localPosition.x, rightposition.localPosition.x, currentProgressValue);
            loadAnimation.transform.localPosition = p;


            //多算一些
            if (currentProgressValue >= 1.1f)
            {
                ShowGameStart();
                return;
            }

            currentRemainderTime -= Time.deltaTime;
            if (currentRemainderTime >= 0.0f)
            {
                UpdateTime();
            }
        }
        
    }

    public override void Dispose()
    {
        base.Dispose();

        //Debug.Log("卸载选择地图界面");
        if (m_Background != null)
        {
            UnityEngine.GameObject.Destroy(m_Background);
            m_Background = null;
        }
        if (LoadObject != null)
        {
            UnityEngine.GameObject.Destroy(LoadObject);
            LoadObject = null;
        }
        if (SelectMap_Remainder != null)
        {
            UnityEngine.GameObject.Destroy(SelectMap_Remainder.gameObject);
            SelectMap_Remainder = null;
        }
        if (Prompt_GameStart != null)
        {
            UnityEngine.GameObject.Destroy(Prompt_GameStart);
            Prompt_GameStart = null;
        }
        
    }
    internal GameObject Prompt_GameStart = null;
    //显示开始游戏
    private void ShowGameStart()
    {
        if (LoadObject != null)
        {
            UnityEngine.GameObject.Destroy(LoadObject);
            LoadObject = null;
        }
        if (SelectMap_Remainder != null)
        {
            UnityEngine.GameObject.Destroy(SelectMap_Remainder.gameObject);
            SelectMap_Remainder = null;
        }
        Prompt_GameStart = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_GameStart.prefab", GameRoot.gameResource);
        GuiPlaneAnimationPlayer ani = GameObject.Find("/UICamera(Clone)/Prompt_GameStart(Clone)/text").GetComponent<GuiPlaneAnimationPlayer>();
        ani.DelegateOnPlayEndEvent += PromptGameStartPlayEnd;
        ani.Play();
        SoundEffectPlayer.Play("startgame.wav");
    }
    private void PromptGameStartPlayEnd()
    {
        if (Prompt_GameStart != null)
        {
            UnityEngine.GameObject.Destroy(Prompt_GameStart);
            Prompt_GameStart = null;
        }
        GameStart();
    }
    //开始游戏
    public void GameStart()
    {
        ((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart(GameSelectMapProcess.currentSelectMapIndex);
        GameRoot.gameProcessControl.ActivateProcess(typeof(GameRaceProcess));
    }

}
