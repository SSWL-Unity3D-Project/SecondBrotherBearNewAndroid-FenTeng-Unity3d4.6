using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAIControllerXiong : PlayerAIController
{
    /*
     * 熊的AI设定
     * 1，首先确认光头强是不是在追我，如果没追我，就开始减速到最小加速度
     * 2,如果光头强盯上我了。我就需要进行一个预警范围，当光头强进入我的预警范围，我就需要往极限加速
     * 3，同时需要以躲避光头强的方向进行调整。
     * **/
    private PlayerAIControllerGuangTouQiang m_GuangTouQiangAi = null;
    protected PlayerAIControllerGuangTouQiang guangTouQiangAi
    {
        get
        {
            if (m_GuangTouQiangAi != null)
                return m_GuangTouQiangAi;
            IParkourPlayer_GuangTouQiang guangtouqiang = ((IParkourControllerScriptCS)(playerController.hControllerScriptCS)).playerGuangTouQiang;
            m_GuangTouQiangAi = (PlayerAIControllerGuangTouQiang)guangtouqiang.playerAIControler;
            return m_GuangTouQiangAi;
        }
    }
    //预警范围
    public float warningRange = 0.2f;
    //抓住后加速
    public float captureAccelerateMinValue = 0.8f;
    public float captureAccelerateDampRate = 1.5f;
    protected float captureAccelerateValue { get { return FTLibrary.Command.FTRandom.NextSingle(captureAccelerateMinValue, 1.0f); } }

    private enum XiongStatus
    {
        Status_Normal,
        Status_Wait,
        Status_Standby,
    }
    private XiongStatus status = XiongStatus.Status_Normal;
    public float waitTime = 1.0f;
    //被抓住的次数
    public int CaptureCount { get; set; }


    private bool IsSelectTurnDirect = false;
    private ParkourPlayerController.PlayerTurnDirect selectTurnDirect;
    private ParkourPlayerController.PlayerTurnDirect SelectOneTurnDirect()
    {
        int v = FTLibrary.Command.FTRandom.Next(3);
        //Debug.Log(v);
        if (v == 0)
        {
            return ParkourPlayerController.PlayerTurnDirect.Turn_Left;
        }
        else if (v == 1)
        {
            return ParkourPlayerController.PlayerTurnDirect.Turn_Right;
        }
        else
        {
            return ParkourPlayerController.PlayerTurnDirect.Turn_Mid;
        }
    }
    protected override void Start()
    {
        base.Start();
        mPlayerAccMinRecord = accelerateMinValue;
        playerController.PlayerSystemForceAccelerateCloseEvent += OnForceAccelerateCloseEvent;
        status = XiongStatus.Status_Wait;
        Invoke("DelayToNormal", waitTime);
    }
    private void OnForceAccelerateCloseEvent(object sender, EventArgs e)
    {
        status = XiongStatus.Status_Wait;
        Invoke("DelayToNormal", waitTime);
    }
    private void DelayToNormal()
    {
        status = XiongStatus.Status_Normal;
    }

    /// <summary>
    /// Ai加速度变化的时间记录信息.
    /// </summary>
    float mLastAiAccTime = 0f;
    public override void AIUpdate()
    {
        if (status == XiongStatus.Status_Normal)
        {
            if (guangTouQiangAi.currentCapturePlayer == playerController &&
                    Mathf.Abs(guangTouQiangAi.playerController.currentPlayerAccelerate - playerController.currentPlayerAccelerate) <= warningRange)
            {
                playerController.playerAccelerateSign = 1.0f;
                //随机选择一个方向躲闪
                if (!IsSelectTurnDirect)
                {
                    IsSelectTurnDirect = true;
                    selectTurnDirect = SelectOneTurnDirect();
                }
                playerController.playerTurnDirect = selectTurnDirect;
            }
            else
            {
                //playerController.playerAccelerateSign = accelerateMinValue;
                if (Time.time - mLastAiAccTime >= 3f)
                {
                    mLastAiAccTime = Time.time;
                    accelerateMinValue = UnityEngine.Random.Range(-0.05f, 0.8f);
                }
                playerController.playerAccelerateSign = Mathf.MoveTowards(playerController.playerAccelerateSign, accelerateMinValue, Time.deltaTime * 3f);
                IsSelectTurnDirect = false;
                playerController.playerTurnDirect = ParkourPlayerController.PlayerTurnDirect.Turn_Mid;
            }
        }
        else if (status == XiongStatus.Status_Wait)
        {
            playerController.playerTurnDirect = ParkourPlayerController.PlayerTurnDirect.Turn_Mid;
        }
        else if (status == XiongStatus.Status_Standby)
        {

        }
    }
    //我与障碍物发生碰撞了
    public override void OnEventHitBrokenObject()
    {
        //UnityEngine.Debug.Log("xiong");
        status = XiongStatus.Status_Standby;
        IsSelectTurnDirect = false;
        playerController.OpenPlayerSystemForceAccelerate(hitDecelerateValue,
                            hitDecelerateDampRate);
        //播放碰撞动画和语言
        playerController.playerAnimator.BrotherBearAniCollide();
    }
    //光头强抓到我了
    public void OnEventGuangTouQiangCaptureMy(IParkourPlayerController guangTouQiang)
    {
        //抓住后加速
        status = XiongStatus.Status_Standby;
        IsSelectTurnDirect = false;
        playerController.OpenPlayerSystemForceAccelerate(captureAccelerateValue,
                            captureAccelerateDampRate);
        //播放被抓住动画和语言
        playerController.playerAnimator.BrotherBearAniCollideArrest();
    }
}
