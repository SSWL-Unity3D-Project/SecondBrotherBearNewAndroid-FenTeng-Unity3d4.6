using System;
using System.Collections.Generic;
using FTLibrary.SerialPortAccess.access;
using UnityEngine;

class SerialPortDeviceAccessJava : SerialPortDeviceAccess
{
        protected string portName = "";
        protected int baudRate = 115200;
        protected int readBufferSize = 2048;
        private AndroidJavaObject jo = null;
        protected bool Isopen = false;
        public override int PortIndex 
        {
            set 
            {
                switch (value)
                {
                    case 0:
                        PortName = "/dev/ttyS0";
                        break;
                    case 1:
                        PortName = "/dev/ttyS1";
                        break;
                    case 2:
                        PortName = "/dev/ttyS2";
                        break;
                    case 3:
                        PortName = "/dev/ttyS3";
                        break;
                    default:
                        PortName = "/dev/ttyS2";
                        break;

                }
            }
        }
        public override string PortName 
        {
            get { return portName; }
            set{portName = value;}
        }
        public override int BaudRate 
        { get{return baudRate;}
            set{baudRate = value;}
        }
        public override bool IsOpen 
        {
            get { return Isopen; } 
        }
        public override int ReadTimeout 
        {
            get { return 0; }
            set {  }
        }
        public override int ReadBufferSize 
        {
            get { return readBufferSize; }
            set{readBufferSize = value;}
        }
        public override int WriteTimeout 
        {
            get { return 0; }
            set{}
        }
        public override int WriteBufferSize 
        {
            get { return 0; }
            set{}
        }
        public SerialPortDeviceAccessJava()
            :base()
        {
            jo = new AndroidJavaObject("com.ss.game.MySerialPort");
        }

        public override void Open() 
        {
            Isopen = jo.Call<bool>("open", portName, baudRate, readBufferSize);
        }
        public override void Close() 
        {
            jo.Call("close");
        }
        public override int Read(byte[] buffer, int offset, int count) 
        {
            byte[] data = jo.Call<byte[]>("recv");
            if (data.Length == 0)
                return 0;
            int copyLength = (data.Length <= count) ? data.Length : count;
            Buffer.BlockCopy(data, 0, buffer, 0, copyLength);
            return copyLength;
        }
        public override void Write(byte[] buffer, int offset, int count) 
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            jo.Call("send", data);
        }
}
