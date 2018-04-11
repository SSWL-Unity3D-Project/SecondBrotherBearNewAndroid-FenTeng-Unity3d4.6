using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
partial class InputDevice : InputDeviceBase
{
    public static bool SystemButton 
    { 
        get 
        { 
            //前进加开火
            return inputDevice.ButtonSystem();
        } 
    }

    public static bool ControlPanelUpButton { get { return inputDevice.ButtonFrontDown(0) || inputDevice.ButtonFrontDown(1); } }
    public static bool ControlPanelDownButton { get { return inputDevice.ButtonLeftSystem(); } }
    public static bool ControlPanelLeftButton { get { return inputDevice.ButtonLeftSystem(); } }
    public static bool ControlPanelRightButton { get { return inputDevice.ButtonRightSystem(); } }
    //针对操作键位较少的产品使用简易键
    public static bool ControlPanelSimpleDownButton { get { return ControlPanelLeftButton; } }
    public static bool ControlPanelSimpleRightButton { get { return ControlPanelRightButton; } }

    public static bool ControlPanelStartButton { get { return inputDevice.ButtonFireDown(0) || inputDevice.ButtonFireDown(1); } }
}
