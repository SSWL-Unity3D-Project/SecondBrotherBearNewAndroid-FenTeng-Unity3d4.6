using System;
using System.Collections.Generic;
using FTLibrary.SerialPortAccess.access;

class SerialPortDeviceAccessConstructorJava : SerialPortDeviceAccessConstructor
{
    public override SerialPortDeviceAccess AllocAccess()
    {
        return new SerialPortDeviceAccessJava();
    }
}
