using UnityEngine;
using System.Collections;

class GameSelectMapProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_SelectMap; } }
    public GameSelectMapProcess()
        : base()
    {

    }

    //背景界面
    internal Transform m_Background = null;
    internal GameObject mapSelectControl = null;
    //左图 选择界面
    internal GameObject LeftMap = null;
    //右图 选择界面
    internal GameObject RightMap = null;
    //当前选择的地图索引
    public static int currentSelectMapIndex;
    //提示玩家的操作
    //条件为，可以进行游戏的玩家，如果P1，P2都可以，则默认为P1控制
    internal GameObject Prompt_SelectMap = null;
    //提示剩余的时间
    internal GuiPlaneAnimationTextAdvanced SelectMap_Remainder = null;
    //决策出来是那个玩家来操作
    internal IParkourPlayer_Xiong.PlayerIndex handlePlayerIndex = IParkourPlayer_Xiong.PlayerIndex.Index_P1;
    //当前界面保留的时间
    internal const int RemainderTime = 15;
    internal float currentRemainderTime = 0.0f;
    internal int prveRemainderTime = 0;
    //初始化函数
    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();

        base.Initialization();
        //Debug.Log("选择地图 初始化");
        GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("FadeScreen.prefab", GameRoot.gameResource);

        //决策应该由那个玩家来操作
        if (((RaceSceneControl)GameRoot.CurrentSceneControl).IsEnterStart(IParkourPlayer_Xiong.PlayerIndex.Index_P1))
        {
            handlePlayerIndex = IParkourPlayer_Xiong.PlayerIndex.Index_P1;
        }
        else if (((RaceSceneControl)GameRoot.CurrentSceneControl).IsEnterStart(IParkourPlayer_Xiong.PlayerIndex.Index_P2))
        {
            handlePlayerIndex = IParkourPlayer_Xiong.PlayerIndex.Index_P2;
        }

        InitUI();
    }

    private void InitUI()
    {
        m_Background = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelectMapBackground.prefab", GameRoot.gameResource).GetComponent<Transform>();
        mapSelectControl = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelecetMapControl.prefab", GameRoot.gameResource);
        LeftMap = GameObject.Find("/UICamera(Clone)/SelecetMapControl(Clone)/PlanAnimation1");
        RightMap = GameObject.Find("/UICamera(Clone)/SelecetMapControl(Clone)/PlanAnimation2");
        currentSelectMapIndex = 0;
        UpdateMap();

        switch (handlePlayerIndex)
        {
            case IParkourPlayer_Xiong.PlayerIndex.Index_P1:
                Prompt_SelectMap = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_SelectMap_Left.prefab", GameRoot.gameResource);
                break;
            case IParkourPlayer_Xiong.PlayerIndex.Index_P2:
                Prompt_SelectMap = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_SelectMap_Right.prefab", GameRoot.gameResource);
                break;
        }
        SelectMap_Remainder = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("SelectMap_Remainder.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationTextAdvanced>();
        currentRemainderTime = RemainderTime;
        UpdateTime();
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
    private void UpdateMap()
    {
        switch (currentSelectMapIndex)
        {
            case 0:
                LeftMap.SetActive(true);
                RightMap.SetActive(false);
                break;
            case 1:
                LeftMap.SetActive(false);
                RightMap.SetActive(true);
                break;
        }
    }
    private void EnterStart()
    {
        if (UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free || UniGameOptionsDefine.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_InsertCoins)
        {
            if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_SelectMap)
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
        if (InputDevice.ButtonLeft((int)handlePlayerIndex))
        {
            currentSelectMapIndex = 0;
            UpdateMap();
        }
        else if (InputDevice.ButtonRight((int)handlePlayerIndex))
        {
            currentSelectMapIndex = 1;
            UpdateMap();
        }
        else if (InputDevice.ButtonEnter((int)handlePlayerIndex))
        {
            GameStart();
            return;
        }

        currentRemainderTime -= Time.deltaTime;
        UpdateTime();
        if (currentRemainderTime < 0.0f)
        {
            GameStart();
            return;
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        //Debug.Log("卸载选择地图界面");
        UnityEngine.GameObject.Destroy(m_Background.gameObject);
        m_Background = null;
        UnityEngine.GameObject.Destroy(mapSelectControl);
        mapSelectControl = null;
        UnityEngine.GameObject.Destroy(Prompt_SelectMap);
        Prompt_SelectMap = null;
        UnityEngine.GameObject.Destroy(SelectMap_Remainder.gameObject);
        SelectMap_Remainder = null;
    }

    //开始游戏
    public void GameStart()
    {
        //((RaceSceneControl)GameRoot.CurrentSceneControl).GameStart(currentSelectMapIndex);
        //GameRoot.gameProcessControl.ActivateProcess(typeof(GameRaceProcess)); 

        GameRoot.gameProcessControl.ActivateProcess(typeof(GameLoadingMapProcess)); 
    }
}
