using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

enum InputMode
{
    Noting = -1,
    SteerWheel = 0,     /*方向盘*/
    JoyStick = 1,     /*手柄*/
    KeyBoard = 2,    /*键盘*/
    DDR_IODevice = 3,     //疯狂飙车IO设备
    Initial_D_5_IODevice = 4, //头5IO设备
    Type_Treasure_IODevice = 5,         //仓鼠的宝藏
    Type_Dragon = 6           //儿童龙机
}

class InputInterface
{
    public InputInterface() { }
    public static string InputModeToString(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.Noting:
                return "default";
            case InputMode.KeyBoard:
                return "keyboard";
            case InputMode.JoyStick:
                return "joystick";
            case InputMode.SteerWheel:
                return "steerwheel";
            case InputMode.DDR_IODevice:
                return "ddr_iodevice";
            case InputMode.Initial_D_5_IODevice:
                return "initial_d_5_iodevice";
            case InputMode.Type_Treasure_IODevice:
                return "treasure_iodevice";
            case InputMode.Type_Dragon:
                return "dragon_iodevice";
        }
        return "";
    }

    public static InputMode InputModeStringToMode(string s)
    {
        switch (s)
        {
            case "default":
                return InputMode.Noting;
            case "keyboard":
                return InputMode.KeyBoard;
            case "joystick":
                return InputMode.JoyStick;
            case "steerwheel":
                return InputMode.SteerWheel;
            case "ddr_iodevice":
                return InputMode.DDR_IODevice;
            case "initial_d_5_iodevice":
                return InputMode.Initial_D_5_IODevice;
            case "treasure_iodevice":
                return InputMode.Type_Treasure_IODevice;
            case "dragon_iodevice":
                return InputMode.Type_Dragon;
        }
        return InputMode.KeyBoard;
    }

    public static InputInterface AllocInputInterface(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.KeyBoard:
                return new KeyBoardInput();
            case InputMode.JoyStick:
                return new KeyBoardInput();
            case InputMode.SteerWheel:
                return new KeyBoardInput();
            case InputMode.Initial_D_5_IODevice:
                return new KeyBoardInput();
            case InputMode.Type_Treasure_IODevice:
                return new KeyBoardInput();
            case InputMode.DDR_IODevice:
                return new DragonIO();
            case InputMode.Type_Dragon:
                return new DragonIO();

        }
        return new KeyBoardInput();
    }
    internal class InputPlayer
    {
        //按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
        public virtual bool ButtonLeft { get { return false; } }
        public virtual bool ButtonRight { get { return false; } }
        public virtual bool ButtonFront { get { return false; } }
        public virtual bool ButtonBack { get { return false; } }

        public virtual bool ButtonLeftDown { get { return false; } }
        public virtual bool ButtonRightDown { get { return false; } }
        public virtual bool ButtonFrontDown { get { return false; } }
        public virtual bool ButtonBackDown { get { return false; } }
        //喷氮键
        public virtual bool ButtonFire { get { return false; } }
        public virtual bool ButtonFireDown { get { return false; } }

        public virtual int InsertCoins { get { return 0; } }

        //按钮灯亮
        public virtual void SetFireLight(bool v) { }
        //控制出蛋马达
        public virtual void SetEggEngine(bool v) { }
        //出蛋判断
        public virtual bool ButtonEgg { get { return false; } }


        public virtual void SetTicketEngine(bool v) { }
        public virtual bool ButtonTicket { get { return false; } }

        //是否按下退出键
        public virtual bool ButtonQuit { get { return false; } }

        //虚拟轴向
        //垂直
        protected float virtualVerticalDamp = 1.0f;
        private float m_VirtualVertical = 0.0f;
        public float virtualVertical { get { return m_VirtualVertical; } }
        //水平
        protected float virtualHorizontalDamp = 1.0f;
        private float m_VirtualHorizontal = 0.0f;
        public float virtualHorizontal { get { return m_VirtualHorizontal; } }

        public virtual void Initialization(FTLibrary.XML.XmlNode player)
        {
            virtualVerticalDamp = Convert.ToSingle(player.Attribute("virtualVerticalDamp"));
            virtualHorizontalDamp = Convert.ToSingle(player.Attribute("virtualHorizontalDamp"));
        }
        public virtual void Update()
        {
            if (ButtonFront)
            {
                m_VirtualVertical = 1.0f;
            }
            else if (ButtonBack)
            {
                m_VirtualVertical = -1.0f;
            }
            else
            {
                m_VirtualVertical = 0.0f;
            }
            if (ButtonLeft)
            {
                m_VirtualHorizontal = UnityEngine.Mathf.Lerp(m_VirtualHorizontal, 1.0f, UnityEngine.Time.deltaTime * virtualHorizontalDamp);
            }
            else if (ButtonRight)
            {
                m_VirtualHorizontal = UnityEngine.Mathf.Lerp(m_VirtualHorizontal, -1.0f, UnityEngine.Time.deltaTime * virtualHorizontalDamp);
            }
            else
            {
                m_VirtualHorizontal = UnityEngine.Mathf.Lerp(m_VirtualHorizontal, 0.0f, UnityEngine.Time.deltaTime * virtualHorizontalDamp);
            }
        }
    }
    protected virtual InputPlayer AllocInputPlayer()
    {
        return new InputPlayer();
    }
    protected InputPlayer[] PlayerList = null;
    public int Players 
    { 
        get { return PlayerList.Length; }
        set
        {
            PlayerList = new InputPlayer[value];
            for (int i = 0; i < value; i++)
            {
                PlayerList[i] = AllocInputPlayer();
            }
        }
    }
    //加载设备的配置信息
    public virtual void Initialization()
    {

    }
    public virtual void Release()
    {

    }
    //周期刷新函数，有些设备需要
    public virtual void Update()
    {
        if (PlayerList != null && PlayerList.Length !=0)
        {
            for (int i = 0; i < PlayerList.Length;i++ )
            {
                PlayerList[i].Update();
            }
        }
    }

    //按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
    public bool ButtonLeft(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonLeft; 
    }
    public bool ButtonRight(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonRight; 
    }
    public bool ButtonFront(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonFront; 
    }
    public bool ButtonBack(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonBack; 
    }

    public bool ButtonLeftDown(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonLeftDown; 
    }
    public bool ButtonRightDown(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonRightDown; 
    }
    public bool ButtonFrontDown(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonFrontDown; 
    }
    public bool ButtonBackDown(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonBackDown; 
    }
    //喷氮键
    public bool ButtonFire(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonFire; 
    }
    public bool ButtonFireDown(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonFireDown; 
    }

    //按钮灯亮
    public  void SetFireLight(int playerIndex,bool v)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return;
        PlayerList[playerIndex].SetFireLight(v); 
    }
    //控制出蛋马达
    public void SetEggEngine(int playerIndex,bool v)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return;
        PlayerList[playerIndex].SetEggEngine(v); 
    }
    //出蛋判断
    public bool ButtonEgg(int playerIndex) 
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonEgg; 
    }


    public void SetTicketEngine(int playerIndex,bool v)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return;
        PlayerList[playerIndex].SetTicketEngine(v); 
    }
    public bool ButtonTicket(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonTicket; 
    }

    public int GetInsertCoins(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return 0;
        return PlayerList[playerIndex].InsertCoins;
    }


    public bool ButtonQuit(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return false;
        return PlayerList[playerIndex].ButtonQuit;
    }

    //用户确定键
    public bool ButtonEnter(int playerIndex) { return ButtonFire(playerIndex); }
    public bool ButtonEnterDown(int playerIndex) { return ButtonFireDown(playerIndex); }

    //虚拟轴向
    //垂直
    public float GetVertical(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return 0.0f;
        return PlayerList[playerIndex].virtualVertical;
    }
    public float GetHorizontal(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= PlayerList.Length)
            return 0.0f;
        return PlayerList[playerIndex].virtualHorizontal;
    }

    //后台关闭所有IO
    public virtual void ShutDownIO() { }

    //系统键
    public virtual bool ButtonSystem() { return false; }
    public virtual bool ButtonLeftSystem() { return false; }
    public virtual bool ButtonRightSystem() { return false; }
}
