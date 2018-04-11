using System;
using System.Collections;
using UnityEngine;
class LuaScriptTest : LuaScriptMonoBehaviourStartCompletion
{
    //获得绑定函数回掉
    protected override LuaScriptController.OnStoreMethod LuaStoreMethod { get { return MyStoreMethod; } }
    private int My_UpdateFuntionRef = 0;
    private void MyStoreMethod()
    {
        Debug.Log("MyStoreMethod");
        My_UpdateFuntionRef = Lua.StoreLuaMethod("my_updatefuntion");
    }
    protected override void Start()
    {
        base.Start();
        int a = Lua.CallLuaMethod_Int32(My_UpdateFuntionRef, 100, 200);
        Debug.Log("a=" + a);
        Debug.Log(Lua.CallLuaGlobalFuntion_Int32("TestFuntion", 100, 200));
    }



}