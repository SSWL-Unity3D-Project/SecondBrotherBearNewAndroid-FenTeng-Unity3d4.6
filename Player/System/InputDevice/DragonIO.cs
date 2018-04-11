
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTLibrary.XML;
using FTLibrary.Command;
using SerialPortAccess.Axis_WaterGun;
using SerialPortAccess;
using UnityEngine;

class DragonIO : InputInterface
{
    public DragonIO()
        : base()
    {

    }

    private int SystemButtonKey = 0;
    private int SystemLeftButtonKey = 0;
    private int SystemRightButtonKey = 0;

    private enum ButtonStatus
    {
        Button_Nothing,
        Button_Down,
        Button_Press,
        Button_Up,
    }


    //左右不在使用按钮，替换为电位器
    //需要设定死区
    internal class InputPlayerIO : InputPlayer
    {
        //private int Left;
        //private int Right;
        private int TurnMidValue;
        private int TurnSilenceValue;
        private int TurnAxisKey;

        private int Front;
        private int Back;
        private int InsertCoinsKey;

        private int Fire;
        private int FireOutput;

        private int EggInput;
        private int EggOutput;

        private int TicketInput;
        private int TicketOutput;

        


        //按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
        private bool IsButtonLeft = false;
        private bool IsButtonRight = false;
        private bool IsButtonFront = false;
        private bool IsButtonBack = false;
        public override bool ButtonLeft { get { return IsButtonLeft; } }
        public override bool ButtonRight { get { return IsButtonRight; } }
        public override bool ButtonFront { get { return IsButtonFront; } }
        public override bool ButtonBack { get { return IsButtonBack; } }

        private bool ButtonLeftDownFlag;
        public override bool ButtonLeftDown
        {
            get
            {
                bool ret = false;
                if (ButtonLeft)
                {
                    if (!ButtonLeftDownFlag)
                    {
                        ButtonLeftDownFlag = true;
                        ret = true;
                    }
                }
                else
                {
                    ButtonLeftDownFlag = false;
                }
                return ret;
            }
        }

        private bool ButtonRightDownFlag;
        public override bool ButtonRightDown
        {
            get
            {
                bool ret = false;
                if (ButtonRight)
                {
                    if (!ButtonRightDownFlag)
                    {
                        ButtonRightDownFlag = true;
                        ret = true;
                    }
                }
                else
                {
                    ButtonRightDownFlag = false;
                }
                return ret;
            }
        }
        private bool ButtonFrontDownFlag;
        public override bool ButtonFrontDown
        {
            get
            {
                bool ret = false;
                if (ButtonFront)
                {
                    if (!ButtonFrontDownFlag)
                    {
                        ButtonFrontDownFlag = true;
                        ret = true;
                    }
                }
                else
                {
                    ButtonFrontDownFlag = false;
                }
                return ret;
            }
        }
        private bool ButtonBackDownFlag;
        public override bool ButtonBackDown
        {
            get
            {
                bool ret = false;
                if (ButtonBack)
                {
                    if (!ButtonBackDownFlag)
                    {
                        ButtonBackDownFlag = true;
                        ret = true;
                    }
                }
                else
                {
                    ButtonBackDownFlag = false;
                }
                return ret;
            }
        }
        //喷氮键
        private bool IsButtonFire = false;
        public override bool ButtonFire { get { return IsButtonFire; } }
        private bool ButtonFireDownFlag;
        public override bool ButtonFireDown
        {
            get
            {
                bool ret = false;
                if (ButtonFire)
                {
                    if (!ButtonFireDownFlag)
                    {
                        ButtonFireDownFlag = true;
                        ret = true;
                    }
                }
                else
                {
                    ButtonFireDownFlag = false;
                }
                return ret;
            }
        }

        public override int InsertCoins { get { return DragonIO.serialPortInput.GetCounterAccessReset(InsertCoinsKey); } }


        //按钮灯亮
        public override void SetFireLight(bool v)
        { serialPortInput.SetCommanderAccess(FireOutput, v); }
        //控制出蛋马达
        public override void SetEggEngine(bool v)
        { serialPortInput.SetCommanderAccess(EggOutput, v); }
        //出蛋判断
        public override bool ButtonEgg { get { return DragonIO.serialPortInput.GetCounterAccessReset(EggInput) != 0; } }


        public override void SetTicketEngine(bool v)
        { serialPortInput.SetCommanderAccess(TicketOutput, v); }
        public override bool ButtonTicket { get { return DragonIO.serialPortInput.GetCounterAccessReset(TicketInput) != 0; } }


        public override void Initialization(FTLibrary.XML.XmlNode player)
        {
            base.Initialization(player);
            //Left = Convert.ToInt32(player.Attribute("leftValue"));
            //Right = Convert.ToInt32(player.Attribute("rightValue"));
            TurnMidValue = Convert.ToInt32(player.Attribute("TurnMidValue"));
            TurnSilenceValue = Convert.ToInt32(player.Attribute("TurnSilenceValue"));
            TurnAxisKey = Convert.ToInt32(player.Attribute("TurnAxisKey"));
            Front = Convert.ToInt32(player.Attribute("frontValue"));
            Back = Convert.ToInt32(player.Attribute("backValue"));
            InsertCoinsKey = Convert.ToInt32(player.Attribute("InsertCoinsKey"));

            XmlNode node = player.SelectSingleNode("FireValue");

            Fire = Convert.ToInt32(node.Attribute("inputValue"));
            FireOutput = Convert.ToInt32(node.Attribute("outputValue"));

            node = player.SelectSingleNode("Egg");
            EggInput = Convert.ToInt32(node.Attribute("inputValue"));
            EggOutput = Convert.ToInt32(node.Attribute("outputValue"));

            node = player.SelectSingleNode("Ticket");
            TicketInput = Convert.ToInt32(node.Attribute("inputValue"));
            TicketOutput = Convert.ToInt32(node.Attribute("outputValue"));
        }
        public override void Update()
        {
            //IsButtonLeft = DragonIO.serialPortInput.GetButtonAccess(Left);
            //IsButtonRight = DragonIO.serialPortInput.GetCounterAccessReset(Right) != 0;
            int value = DragonIO.serialPortInput.GetAxisAccessValue(TurnAxisKey);
            if (value < (TurnMidValue - TurnSilenceValue))
            {
                IsButtonLeft = true;
                IsButtonRight = false;
            }
            else if (value > (TurnMidValue + TurnSilenceValue))
            {
                IsButtonLeft = false;
                IsButtonRight = true;
            }
            else
            {
                IsButtonLeft = false;
                IsButtonRight = false;
            }
            IsButtonFront = DragonIO.serialPortInput.GetCounterAccessReset(Front) != 0;
            IsButtonBack = DragonIO.serialPortInput.GetCounterAccessReset(Back) != 0;
            IsButtonFire = DragonIO.serialPortInput.GetCounterAccessReset(Fire) != 0;

            //DragonIO.serialPortInput.GetButtonAccess(Fire)
        }
    }
    protected override InputPlayer AllocInputPlayer()
    {
        return new InputPlayerIO();
    }
    public static SerialPortInput serialPortInput = null;
    //private static SerialPortInputTrack serialPortInputTrack = null;
    private int ComPortIndex = 0;
    public override void Initialization()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_PublicXmlFile("GameOptions.xml");
        XmlNode root = doc.SelectSingleNode("GameOptions");
        XmlNode node = root.SelectSingleNode("GameInputMode");
        ComPortIndex = (int)FTConvert.AutoToInt32(node.Attribute("iocom"));

        doc = GameRoot.gameResource.LoadResource_PublicXmlFile("Inputconfig.xml");
        root = doc.SelectSingleNode("inputconfig");
        Players = Convert.ToInt32(root.Attribute("players"));

        node = root.SelectSingleNode("IO");
        SystemButtonKey = Convert.ToInt32(node.Attribute("sytemValue"));
        SystemLeftButtonKey = Convert.ToInt32(node.Attribute("systemLeftValue"));
        SystemRightButtonKey = Convert.ToInt32(node.Attribute("systemRightValue"));

        XmlNodeList playerNodeList = node.SelectNodes("player");
        for (int i = 0; i < playerNodeList.Count; i++)
        {
            PlayerList[i].Initialization(playerNodeList[i]);
        }


        serialPortInput = null;
        serialPortInput = new SerialPortInput();
        //serialPortInputTrack = new SerialPortInputTrack();
        //serialPortInput.traceObject = serialPortInputTrack;

        if (!serialPortInput.Initialization(SerialPortAccessType.Type_DDR_Android, ComPortIndex,
                                new SerialPortDeviceAccessConstructorJava()))
        {
            throw new Exception("Initialization DDR IO Device Err!");
        }
    }
    //周期刷新函数，有些设备需要
    public override void Update()
    {
        if (serialPortInput != null)
        {
            serialPortInput.UpdateDevice();
        }
        base.Update();
        if (DragonIO.serialPortInput.GetButtonAccess(SystemLeftButtonKey))
        {
            if (buttonLeftSystemStatus == ButtonStatus.Button_Nothing)
            {
                buttonLeftSystemStatus = ButtonStatus.Button_Down;
            }
            else if (buttonLeftSystemStatus == ButtonStatus.Button_Down)
            {
                buttonLeftSystemStatus = ButtonStatus.Button_Press;
            }
        }
        else
        {
            if (buttonLeftSystemStatus == ButtonStatus.Button_Down ||
                buttonLeftSystemStatus == ButtonStatus.Button_Press)
            {
                buttonLeftSystemStatus = ButtonStatus.Button_Up;
            }
            else if (buttonLeftSystemStatus == ButtonStatus.Button_Up)
            {
                buttonLeftSystemStatus = ButtonStatus.Button_Nothing;
            }
        }



        if (DragonIO.serialPortInput.GetButtonAccess(SystemRightButtonKey))
        {
            if (buttonRightSystemStatus == ButtonStatus.Button_Nothing)
            {
                buttonRightSystemStatus = ButtonStatus.Button_Down;
            }
            else if (buttonRightSystemStatus == ButtonStatus.Button_Down)
            {
                buttonRightSystemStatus = ButtonStatus.Button_Press;
            }
        }
        else
        {
            if (buttonRightSystemStatus == ButtonStatus.Button_Down ||
                buttonRightSystemStatus == ButtonStatus.Button_Press)
            {
                buttonRightSystemStatus = ButtonStatus.Button_Up;
            }
            else if (buttonRightSystemStatus == ButtonStatus.Button_Up)
            {
                buttonRightSystemStatus = ButtonStatus.Button_Nothing;
            }
        }
    }




    private ButtonStatus buttonLeftSystemStatus = ButtonStatus.Button_Nothing;
    private ButtonStatus buttonRightSystemStatus = ButtonStatus.Button_Nothing;
    //系统键
    public override bool ButtonSystem() { return DragonIO.serialPortInput.GetButtonAccess(SystemButtonKey);/*DragonIO.serialPortInput.GetCounterAccessReset(SystemButtonKey) != 0*/; }
    public override bool ButtonLeftSystem() { return buttonLeftSystemStatus == ButtonStatus.Button_Down; }
    public override bool ButtonRightSystem() { return buttonRightSystemStatus == ButtonStatus.Button_Down; }

    //关闭IO
    public override void ShutDownIO()
    {
        for (int i = 0; i < 30; i++)
        {
            serialPortInput.SetCommanderAccess(i, false);
        }
    }


}


