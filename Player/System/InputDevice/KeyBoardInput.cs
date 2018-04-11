using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FTLibrary.XML;
using FTLibrary.Command;

class KeyBoardInput : InputInterface
{
    public KeyBoardInput()
        : base()
    {

    }
    internal class InputPlayerKeyBoard:InputPlayer
    {
        private KeyCode Left;
        private KeyCode Right;
        private KeyCode Front;
        private KeyCode Back;
        private KeyCode Fire;
        private KeyCode InsertCoin;

        public enum OttKeyCode
        {
            Up = 273,
            Down = 274,
            Left = 276,
            Right = 275,
            OK = 330,
            OK1 = 10,
            Menu = 319,
            Back = 27,
            Joy_Sel = 338,
            Joy_Start = 339,
            Joy_1 = 330,
            Joy_3 = 332,
            Joy_4 = 333,
            Joy_2 = 331,
            Joy_R1 = 334,
            Joy_R2 = 336,
            Joy_L1 = 335,
            Joy_L2 = 337,
        }
        private const float OttJoyHorizontal_Up = 1.0f;
        private const float OttJoyHorizontal_Down = -1.0f;
        private const float OttJoyHorizontal_Left = -1.0f;
        private const float OttJoyHorizontal_Right = 1.0f;

        //按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
        //public override bool ButtonLeft { get { return Input.GetKey(Left); } }
        //public override bool ButtonRight { get { return Input.GetKey(Right); } }
        //public override bool ButtonFront { get { return Input.GetKey(Front); } }
        //public override bool ButtonBack { get { return Input.GetKey(Back); } }

        public override bool ButtonLeft
        {
            get
            {
                if (Input.GetKey(Left))
                {
                    return true;
                }
                else if (Input.GetKey((KeyCode)OttKeyCode.Left))
                {
                    return true;
                }
                else if (Input.GetAxis("Horizontal") == OttJoyHorizontal_Left)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ButtonRight
        {
            get
            {
                if (Input.GetKey(Right))
                {
                    return true;
                }
                else if (Input.GetKey((KeyCode)OttKeyCode.Right))
                {
                    return true;
                }
                else if (Input.GetAxis("Horizontal") == OttJoyHorizontal_Right)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ButtonFront
        {
            get
            {
                if (Input.GetKeyDown(Front))
                {
                    return true;
                }
                else if (Input.GetKeyDown((KeyCode)OttKeyCode.Up))
                {
                    return true;
                }
                else if (Input.GetAxis("Horizontal") == OttJoyHorizontal_Up)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ButtonBack
        {
            get
            {
                if (Input.GetKeyDown(Back))
                {
                    return true;
                }
                else if (Input.GetKeyDown((KeyCode)OttKeyCode.Down))
                {
                    return true;
                }
                else if (Input.GetAxis("Horizontal") == OttJoyHorizontal_Down)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //是否按下退出键
        public override bool ButtonQuit
        {
            get
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    return true;
                }
                else if (Input.GetKeyDown((KeyCode)OttKeyCode.Back))
                {
                    return true;
                }
                return false;
            }
        }

        private bool IsPressOkKey
        {
            get
            {
                if (Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    return true;
                }
                else if (Input.GetKey((KeyCode)OttKeyCode.OK) ||
                        Input.GetKey((KeyCode)OttKeyCode.OK1))
                {
                    return true;
                }
                else if (Input.GetKey((KeyCode)OttKeyCode.Joy_Start) ||
                        Input.GetKey((KeyCode)OttKeyCode.Joy_1) ||
                        Input.GetKey((KeyCode)OttKeyCode.Joy_2) ||
                        Input.GetKey((KeyCode)OttKeyCode.Joy_3) ||
                        Input.GetKey((KeyCode)OttKeyCode.Joy_4))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool ButtonLeftDown { get { return Input.GetKeyDown(Left); } }
        public override bool ButtonRightDown { get { return Input.GetKeyDown(Right); } }
        public override bool ButtonFrontDown { get { return Input.GetKeyDown(Front); } }
        public override bool ButtonBackDown { get { return Input.GetKeyDown(Back); } }
        //喷氮键
        public override bool ButtonFire { get { return Input.GetKey(Fire); } }
        public override bool ButtonFireDown { get { return Input.GetKeyDown(Fire); } }

        //投币
        private int InsertCoinsConter = 0;
        public override int InsertCoins
        {
            get
            {
                int ret = InsertCoinsConter;
                InsertCoinsConter = 0;
                return ret;
            }
        }


        public override void Initialization(FTLibrary.XML.XmlNode player)
        {
            base.Initialization(player);
            Left = (KeyCode)Convert.ToInt32(player.Attribute("leftValue"));
            Right = (KeyCode)Convert.ToInt32(player.Attribute("rightValue"));
            Front = (KeyCode)Convert.ToInt32(player.Attribute("frontValue"));
            Back = (KeyCode)Convert.ToInt32(player.Attribute("backValue"));
            InsertCoin = (KeyCode)Convert.ToInt32(player.Attribute("InsertCoin"));

            Fire = (KeyCode)Convert.ToInt32(player.Attribute("fireValue"));
        }
        public override void Update()
        {
            if (Input.GetKeyDown(InsertCoin))
            {
                InsertCoinsConter += 1;
            }
        }
    }
    protected override InputPlayer AllocInputPlayer()
    {
        return new InputPlayerKeyBoard();
    }
    private KeyCode SystemButton;
    private KeyCode SystemLeftButton;
    private KeyCode SystemRightButton;
    public override void Initialization()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_PublicXmlFile("Inputconfig.xml");
        XmlNode root = doc.SelectSingleNode("inputconfig");
        Players = Convert.ToInt32(root.Attribute("players"));

        XmlNode node = root.SelectSingleNode("keyboard");
        SystemButton = (KeyCode)Convert.ToInt32(node.Attribute("sytemValue"));
        SystemLeftButton = (KeyCode)Convert.ToInt32(node.Attribute("systemLeftValue"));
        SystemRightButton = (KeyCode)Convert.ToInt32(node.Attribute("systemRightValue"));
        XmlNodeList playerNodeList = node.SelectNodes("player");
        for (int i = 0; i < playerNodeList.Count;i++ )
        {
             PlayerList[i].Initialization(playerNodeList[i]);
        }
    }
    //系统键
    public override bool ButtonSystem() { return Input.GetKeyDown(SystemButton); }
    public override bool ButtonLeftSystem() { return Input.GetKeyDown(SystemLeftButton); }
    public override bool ButtonRightSystem() { return Input.GetKeyDown(SystemRightButton); }

    public override void Update()
    {
        base.Update();
        
    }
}
