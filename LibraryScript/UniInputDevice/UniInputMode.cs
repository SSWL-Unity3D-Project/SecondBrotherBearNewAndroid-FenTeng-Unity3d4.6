using System;
static class UniInputMode
{
    public enum InputMode
    {
        KeyBoard = 0,           /*键盘*/
        DDR_IODevice = 1,       //IO设备
        SteerWheel = 2,         /*方向盘*/
        JoyStick = 3,           /*手柄*/
        TouchScreen=4           //触摸屏幕
        
    }
    public static InputMode InputModeStringToInputMode(string name)
    {
        switch(name)
        {
            case "KeyBoard": //键盘 
                return InputMode.KeyBoard;
            case "DDR_IODevice": //IO设备 
                return InputMode.DDR_IODevice;
            case "SteerWheel": //方向盘 
                return InputMode.SteerWheel;
            case "JoyStick": //手柄 
                return InputMode.JoyStick;
            case "TouchScreen": //触摸屏
                return InputMode.TouchScreen;
        }
        return InputMode.KeyBoard;
    }
    public static bool IsSupportInputMode(InputMode mode)
    {
        return (mode == InputMode.KeyBoard || mode == InputMode.DDR_IODevice);
    }
}


