using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using UnityEngine;

/*
 * 一定要注意的问题：
 * 1，不同的界面中如果注册了代理函数，请自主在界面释放的时候注销代理函数，否则会导致内存泄露
 *    提高的解除全部代码尽量别用
 *    
 * 2，提供的关闭全部灯函数，尽量别用，自己控制不同灯的亮灭
 * 
 * 
 * 
 * */

//映射模式
enum InputDeviceMapMode
{
    Mode_KeyBoard,
    Mode_IO,
    Mode_Others
}

partial class InputDevice : InputDeviceBase
{
    //对象以组件的方式只是使用游戏的刷新周期
    //实际上所有的输入响应都是静态的
    //也就是在游戏开始的时候选择好后就不会被更改了
    public static InputMode currentInputMode = InputMode.KeyBoard;
    public static InputDeviceMapMode mapMode
    {
        get
        {
            if (currentInputMode == InputMode.SteerWheel ||
                currentInputMode == InputMode.DDR_IODevice ||
                currentInputMode == InputMode.Initial_D_5_IODevice ||
                currentInputMode == InputMode.Type_Treasure_IODevice ||
                currentInputMode == InputMode.Type_Dragon)
            {
                return InputDeviceMapMode.Mode_IO;
            }
            else if (currentInputMode == InputMode.JoyStick ||
                        currentInputMode == InputMode.KeyBoard)
            {
                return InputDeviceMapMode.Mode_KeyBoard;
            }
            return InputDeviceMapMode.Mode_KeyBoard;
        }
    }

    protected static InputInterface inputDevice = null;

    public static void Initialization()
    {

        XmlDocument doc = GameRoot.gameResource.LoadResource_PublicXmlFile("GameOptions.xml");
        XmlNode root = doc.SelectSingleNode("GameOptions");
        XmlNode node = root.SelectSingleNode("GameInputMode");
        string workMode = node.Attribute("mode");
        currentInputMode = InputInterface.InputModeStringToMode(workMode);
        inputDevice = InputInterface.AllocInputInterface(currentInputMode);
        if (inputDevice == null)
            throw new Exception("unknow input device!");
        inputDevice.Initialization();

        InputDevice.AutoSleepTime_Player[0] = UniGameOptionsDefine.AutoSleepTime;
        InputDevice.AutoSleepTime_Player[1] = UniGameOptionsDefine.AutoSleepTime;
    }

    public static void Release()
    {
        if (inputDevice != null)
        {
            inputDevice.Release();
            inputDevice = null;
        }
    }

    //停止设备响应锁
    private static int m_StopLocker = 0;
    //如果是usb设备需要使用延迟锁
    //因为在切换场景的时候窗口失去焦点，会接受油门，刹车的响应,有
    private static FTLibrary.Time.TimeLocker usbDeviceLocker = new FTLibrary.Time.TimeLocker(5000);

    public static bool StopLocker
    {
        get
        {
            if (m_StopLocker != 0)
                return true;
            if (InputDevice.currentInputMode == InputMode.JoyStick ||
                        InputDevice.currentInputMode == InputMode.SteerWheel)
            {
                return usbDeviceLocker.IsLocked;
            }
            return false;
        }
        set
        {
            if (value)
            {
                m_StopLocker += 1;
                if (InputDevice.currentInputMode == InputMode.JoyStick ||
                        InputDevice.currentInputMode == InputMode.SteerWheel)
                {
                    ////清除确定和刹车标志
                    //SelectDetermine_DownFlag = 0;
                    //SelectCancel_DownFlag = 0;
                }
            }
            else
            {
                m_StopLocker -= 1;
                if (m_StopLocker == 0)
                {
                    if (InputDevice.currentInputMode == InputMode.JoyStick ||
                        InputDevice.currentInputMode == InputMode.SteerWheel)
                    {
                        usbDeviceLocker.IsLocked = true;
                        ////清除确定和刹车标志
                        //SelectDetermine_DownFlag = 0;
                        //SelectCancel_DownFlag = 0;
                    }
                }
            }
        }
    }

    //按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
    public static bool ButtonLeft(int playerIndex) { return inputDevice.ButtonLeft(playerIndex); }
    public static bool ButtonRight(int playerIndex) { return inputDevice.ButtonRight(playerIndex); }
    public static bool ButtonFront(int playerIndex) { return inputDevice.ButtonFront(playerIndex); }
    public static bool ButtonBack(int playerIndex) { return inputDevice.ButtonBack(playerIndex); }

    public static bool ButtonLeftDown(int playerIndex) { return inputDevice.ButtonLeftDown(playerIndex); }
    public static bool ButtonRightDown(int playerIndex) { return inputDevice.ButtonRightDown(playerIndex); }
    public static bool ButtonFrontDown(int playerIndex) { return inputDevice.ButtonFrontDown(playerIndex); }
    public static bool ButtonBackDown(int playerIndex) { return inputDevice.ButtonBackDown(playerIndex); }
    //喷氮键
    public static bool ButtonFire(int playerIndex) { return inputDevice.ButtonFire(playerIndex); }
    public static bool ButtonFireDown(int playerIndex) { return inputDevice.ButtonFireDown(playerIndex); }
    //用户确定键
    public static bool ButtonEnter(int playerIndex) { return ButtonFire(playerIndex); }
    public static bool ButtonEnterDown(int playerIndex) { return ButtonFireDown(playerIndex); }

    //虚拟轴向
    //垂直
    public static float GetVertical(int playerIndex) { return inputDevice.GetVertical(playerIndex); }
    public static float GetHorizontal(int playerIndex) { return inputDevice.GetHorizontal(playerIndex); }
    //转向
    public static bool TurnLeft(int index) { return ButtonLeft(index); }
    public static bool TurnRight(int index) { return ButtonRight(index); }

    //测试时间累计
    private static float[] RotateTime = new float[2];
    //计算出来的测速
    private static float[] RotateSpeed = new float[2];
    //准备进行测速计算
    public static void ReadyAccountRotateSpeed(int index)
    {
        RotateTime[index] = 0.0f;
        RotateSpeed[index] = 0.0f;
    }
    /*
     * 计算方法为：
     * 1，计算上次按下和这次按下直接的间隔时间，计算成每分钟的转速
     * 2，如果在没有按下的情况下，依然计算转速，当转速低于上一次有效计算出来的转速
     * 则用新的转速代替，只到转速为零
     * 
     * */
    private static void AccountRotateSpeed(int index)
    {
        RotateTime[index] += Time.deltaTime;
        if ((ButtonFront(index) || ButtonBack(index)))
        {
            //计算每分钟转速
            RotateSpeed[index] = (1.0f/ RotateTime[index]) * 60.0f;
            //时间重新计算
            RotateTime[index] = 0.0f;
        }
        else
        {
            //假如当前触发按下模拟计算出转速
            float tspeed = (1.0f / RotateTime[index]) * 60.0f;
            //模拟计算的转速已经小于之前测试的转速则开始衰减
            if (tspeed < RotateSpeed[index])
            {
                RotateSpeed[index] = tspeed;
            }
        }
    }
    //加速
    public static float Accelerate(int index) 
    { 
        //使用测速处理，计算出每分钟多少转，根据这个转速来计算出加速值
        AccountRotateSpeed(index);
        //if (index == 0)
        //{
        //    Debug.Log(RotateSpeed[index]);
        //}
        return Mathf.Clamp(RotateSpeed[index] / UniGameOptionsDefine.MaxRotateSpeed, 0.0f, 1.0f);
    }


    //按钮灯亮
    public static void SetFireLight(int playerIndex, bool v)
    {
        inputDevice.SetFireLight(playerIndex, v);
    }
    //控制出蛋马达
    public static void SetEggEngine(int playerIndex, bool v)
    {
        inputDevice.SetEggEngine(playerIndex,v);
    }
    //出蛋判断
    public static bool ButtonEgg(int playerIndex)
    {
        return inputDevice.ButtonEgg(playerIndex);
    }


    public static void SetTicketEngine(int playerIndex, bool v)
    {
        inputDevice.SetTicketEngine(playerIndex,v);
    }
    public static bool ButtonTicket(int playerIndex)
    {
        return inputDevice.ButtonTicket(playerIndex);
    }



    //后台关闭IO
    public static void ShutDownIO()
    {
        inputDevice.ShutDownIO();
    }

    //1P彩蛋机自停
    private static int[] AutoSleepTime_Player = new int[2];
    private static float[] TmpTime_Player = new float[2];
  
    //1P彩票机自停
    private static float[] TmpTicketTime_Player = new float[2];
    



    //玩家1出彩蛋数量
    private static int[] EggNums_Player = new int[2];
    //玩家1是否开启出蛋
    private static bool[] IsEngine_Player = new bool[2];
   
    //玩家1出票数量
    private static int[] TicketNum_Player = new int[2];
    //玩家1是否开启出票
    private static bool[] IsTicketEngin_Player = new bool[2];
   

    //是否显示彩票报警1P
    public static bool[] IsTicketWarning_Player = new bool[2];

    //设置分数
    public static void SetEggNums(int index, int nums)
    {
        EggNums_Player[index] += nums;
    }

    //设置出票分数
    public static void SetTicketNum(int index, int nums)
    {
        TicketNum_Player[index] += nums;
    }




    //投币
    public delegate void DeviceRespondDelegate_1(int value,IParkourPlayer_Xiong.PlayerIndex index);
    public static DeviceRespondDelegate_1 delegateInsertNewCoins = null;

    //退出锁，当为ture的时候显示询问是否再按一次退出
    public static FTLibrary.Time.TimeLocker quitLocker = new FTLibrary.Time.TimeLocker(5000);


    private static void CheckEggEngine(int index)
    {
        //1P
        //判断是否要有要出的彩蛋
        if (EggNums_Player[index] > 0)
        {
            TmpTime_Player[index] -= Time.deltaTime;
            //如果引擎已经关闭 打开出蛋引擎
            if (IsEngine_Player[index] == false)
            {
                inputDevice.SetEggEngine(index, true);
                IsEngine_Player[index] = true;
            }
            //如果引擎打开
            else if (IsEngine_Player[index] == true)
            {
                //如果接收到光眼返回值 则蛋数-1 重置自动睡眠时间
                if (inputDevice.ButtonEgg(index))
                {
                    EggNums_Player[index] -= 1;
                    TmpTime_Player[index] = AutoSleepTime_Player[index];
                }
                //if (Input.GetKeyDown(KeyCode.Alpha2) != false)
                //{
                //    EggNums_R -= 1;
                //    TmpTime_R = AutoSleepTime_R;
                //}
                //如果自动睡眠时间结束 关闭引擎 默认已出一颗扭蛋 重置睡眠时间
                if (TmpTime_Player[index] <= 0)
                {
                    EggNums_Player[index] -= 1;
                    inputDevice.SetEggEngine(index,false);
                    IsEngine_Player[index] = false;
                    TmpTime_Player[index] = AutoSleepTime_Player[index];
                }
            }
        }
        //如果没有需要出的扭蛋
        else
        {
            //如果引擎打开 则需要关闭
            if (IsEngine_Player[index])
            {
                inputDevice.SetEggEngine(index,false);
                IsEngine_Player[index] = false;
                TmpTime_Player[index] = AutoSleepTime_Player[index];
            }
        }

    }

    private static void CheckEggTicket(int index)
    {
         if(TicketNum_Player[index] > 0)
        {
            TmpTicketTime_Player[index] -= Time.deltaTime;
            if(IsTicketEngin_Player[index] == false)
            {
                inputDevice.SetTicketEngine(index,true);
                IsTicketEngin_Player[index] = true;
            }
            //如果引擎开启
            else if(IsTicketEngin_Player[index] == true)
            {
                //如果有值出一票
                if (inputDevice.ButtonTicket(index))
                {
                    if (IsTicketWarning_Player[index])
                    {
                        IsTicketWarning_Player[index] = false;
                    }
                    TicketNum_Player[index] -= 1;
                    TmpTicketTime_Player[index] = AutoSleepTime_Player[index];
                }
                //if (Input.GetKeyDown(KeyCode.Alpha1) != false)
                //{
                //    if (IsTicketWarning_L)
                //    {
                //        IsTicketWarning_L = false;
                //    }
                //    TicketNum_L -= 1;
                //    TicketNum_L = AutoSleepTime_L;
                //}
                //如果睡眠时间结束 关闭引擎
                if(TmpTicketTime_Player[index] <= 0)
                {
                    //TicketNum_L -= 1;
                    if(!IsTicketWarning_Player[index])
                    {
                        IsTicketWarning_Player[index] = true;
                    }
                    inputDevice.SetTicketEngine(index,false);
                    IsTicketEngin_Player[index] = false;
                    TmpTicketTime_Player[index] = AutoSleepTime_Player[index];
                 }
            }
        }
        //如果没有要出的彩票 停止引擎
        else
        {
            if(IsTicketEngin_Player[index] == true)
            {
                if(IsTicketWarning_Player[index])
                {
                    IsTicketWarning_Player[index] = false;
                }
                inputDevice.SetTicketEngine(index,false);
                IsTicketEngin_Player[index] = false;
                TmpTicketTime_Player[index] = AutoSleepTime_Player[index];
            }
        }

    }

    private static void UpdateInsertCoins(int index)
    {
        //处理投币数
        //获取这周期头的币数
        int coins = inputDevice.GetInsertCoins(index);
        if (coins != 0)//说明有投币
        {
            if (delegateInsertNewCoins != null)
            {
                delegateInsertNewCoins(coins, (IParkourPlayer_Xiong.PlayerIndex)index);//参数为本次投的币数
            }
        }
    }

    public static void InputDeviceUpdate()
    {
        if (inputDevice == null)
        {
            return;
        }
        inputDevice.Update();
        //处理投币数
        //获取这周期头的币数
        UpdateInsertCoins(0);
        UpdateInsertCoins(1);
        if (inputDevice.ButtonQuit(0))
        {
            if (quitLocker.IsLocked)
            {
                UnityEngine.Application.Quit();
            }
            else
            {
                quitLocker.IsLocked = true;
            }
        }

        if (UniGameOptionsDefine.GiftModeFunc == UniGameOptionsFile.GiftMode.Egg)
        {
            //1P
            //判断是否要有要出的彩蛋
            CheckEggEngine(0);
            //2P
            //判断是否要有要出的彩蛋
            CheckEggEngine(1);
        }
        else if(UniGameOptionsDefine.GiftModeFunc == UniGameOptionsFile.GiftMode.Ticket)
        {
            CheckEggTicket(0);
            CheckEggTicket(1);
        }

    }
    protected virtual void Update()
    {
        InputDevice.InputDeviceUpdate();
    }




    private static int[] PlayerScores = new int[2];
    //public static void ClearPlayerScores()
    //{
    //    PlayerScores[0] = 0;
    //    PlayerScores[1] = 0;
    //}
    public static void PlayerAddScores(int index, int score)
    {
        PlayerScores[index] += score;
        //出蛋
        if (UniGameOptionsDefine.GiftModeFunc == UniGameOptionsFile.GiftMode.Egg)
        {
            if (PlayerScores[index] >= UniGameOptionsDefine.PerEggScore)
            {
                InputDevice.SetEggNums(index, 1);
                PlayerScores[index] -= UniGameOptionsDefine.PerEggScore;
            }
        }
        //出票
        else if (UniGameOptionsDefine.GiftModeFunc == UniGameOptionsFile.GiftMode.Ticket)
        {
            if (PlayerScores[index] >= UniGameOptionsDefine.PerTicketScore)
            {
                InputDevice.SetTicketNum(index, 1);
                PlayerScores[index] -= UniGameOptionsDefine.PerTicketScore;
            }
        }
    }
}
