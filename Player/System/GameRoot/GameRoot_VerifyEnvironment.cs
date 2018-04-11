//#define _Develop
//#define _Release
//#define _IgnoreVerify
/*
 * 旧版为32寸版
 * #define _SubVersion_32
 * 新加分支版本 42寸大屏幕版本
 * #define _SubVersion_42
 * */
using System;
using UnityEngine;
using System.Collections.Generic;
using FTLibrary.EliteDevice;

//#if _IgnoreVerify
//partial class GameRoot
//{
////弹出报错提示框
//    public static void Error(string msg)
//    {
//#if _Develop
//        Debug.Log(msg);
//#endif //_Develop
//        ErrMessageBox.ShowErrMessage(msg);
//    }
//    public static void ErrorClearFirst(string msg)
//    {
//#if _Develop
//        Debug.Log(msg);
//#endif //_Develop
//        ErrMessageBox.ClearFirstMessage(msg);
//    }
//    public static List<string> non_fatal_error_list = null;
//    public static void NonFatalErro(string msg)
//    {
//        if (non_fatal_error_list == null)
//            non_fatal_error_list = new List<string>(8);
//        non_fatal_error_list.Add(msg);
//    }
//}
//#else
partial class GameRoot
{
    private const uint uiDeveloperID = 0x53303036;
    private const string userPassword = "7FC9F63076294067BB43CEE350B5AEAF";
    private const string productId = "31914EBC5187405B930E90FDF5D61125";
    private const string loginPassword = "37C67DB5143449C3B4B9E9F8E3BB4486";


    //弹出报错提示框
    public static void Error(string msg)
    {
#if _Develop
        Debug.Log(msg);
#endif //_Develop
        ErrMessageBox.ShowErrMessage(msg);
    }
    public static void ErrorClearFirst(string msg)
    {
#if _Develop
        Debug.Log(msg);
#endif //_Develop
        ErrMessageBox.ClearFirstMessage(msg);
    }
    public static List<string> non_fatal_error_list = null;
    public static void NonFatalErro(string msg)
    {
        if (non_fatal_error_list == null)
            non_fatal_error_list = new List<string>(8);
        non_fatal_error_list.Add(msg);
    }

    public static VerifyEnvironmentObj m_VerifyEnvironmentObj = null;
    public static void StartActivation_Environment()
    {
        if (m_VerifyEnvironmentObj == null)
        {
            GameObject obj = new GameObject("VerifyEnvironmentObj");
            m_VerifyEnvironmentObj = (VerifyEnvironmentObj)obj.AddComponent(typeof(VerifyEnvironmentObj));
        }
        m_VerifyEnvironmentObj.StartGameVerify();
        //Initialization();
    }
    public static bool Activation_Environment()
    {
        //如果不在编辑器内隐藏鼠标
        if (!Application.isEditor)
        {
            //隐藏鼠标
            Screen.showCursor = false;
        }
        try
        {
            if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
            {
                EliteDevice.SetEliteDeviceAccess(new EliteDeviceAccessJava());
            }
            else
            {
                EliteDevice.SetEliteDeviceAccess(new EliteDeviceAccessCShape());
            }
            if (!EliteDevice.ActiveDevice(uiDeveloperID,
                                            userPassword,
                                            productId,
                                            loginPassword))
            {
                EliteDevice.CloseDevice();
                throw new Exception("尝试激活加密设备时发生错误!");
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.Log("系统激活错误： " + ex.ToString());
            return false;
        }
    }
    //完整验证当前环境
    public static void CompleteVerify_Current_Environment()
    {
        verifyEnvironment.CompleteVerify_Environment();
    }
}
//#endif//_IgnoreVerify