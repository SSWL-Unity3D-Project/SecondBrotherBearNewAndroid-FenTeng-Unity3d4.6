using System;
using System.Collections.Generic;
using UnityEngine;
public class IParkourPlayer_Camera : IParkourPlayerController
{
    public override PlayerType playerType { get { return PlayerType.Type_Camera; } }
    //是否被忽略
    public override bool IsIgnore { get { return true; } }
    //这里需要控制摄像机
    public Camera camera = null;
}
