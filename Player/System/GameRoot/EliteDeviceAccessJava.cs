using System;
using System.Collections.Generic;
using FTLibrary.EliteDevice;
using UnityEngine;

class EliteDeviceAccessJava : EliteDeviceAccess
{
    private AndroidJavaObject jo = null;
    public EliteDeviceAccessJava()
            :base()
    {
        jo = new AndroidJavaObject("com.ss.game.MyElite5Access");
    }
    public int AccessTest(int a,int b)
    {
        return jo.Call<int>("AccessTest", a, b);
    }
    public override void Device_SecurityInterfaceProc(byte[] input, uint inputLength, byte[] output, out uint outputLength)
    {
        outputLength = 0;
        byte[] data = jo.Call<byte[]>("AccessFromJni",input);
        if (data == null)
            return;
        outputLength = (uint)data.Length;
        Array.Copy(data, output, data.Length);
    }
}

