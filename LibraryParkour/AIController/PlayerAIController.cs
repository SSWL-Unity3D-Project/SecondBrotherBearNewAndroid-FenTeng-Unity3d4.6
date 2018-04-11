using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAIController : MonoBehaviourIgnoreGui
{
    public IParkourPlayerController playerController;
    //最小加速度
    public float accelerateMinValue = 0.0f;
    //碰撞减速
    public float hitDecelerateMaxValue = 0.3f;
    public float hitDecelerateDampRate = 3.0f;
    protected float hitDecelerateValue { get { return FTLibrary.Command.FTRandom.NextSingle(accelerateMinValue, hitDecelerateMaxValue); } }
    protected virtual void Start()
    {
        
    }
    public virtual void AIUpdate()
    {

    }
    //我与障碍物发生碰撞了
    public virtual void OnEventHitBrokenObject()
    {
      
    }
}
