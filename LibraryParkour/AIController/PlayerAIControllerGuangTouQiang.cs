using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAIControllerGuangTouQiang : PlayerAIController
{
    /*
     * 光头强的AI说明：
     * 1,光头强只要处于正常状态就选择最近的一个熊向这个熊靠近
     * 2,如果超过极限加速度就需要减速
     * 3，如果抓到一只熊就需要离开减速一下。
     * 4，如果与障碍物发生碰撞了也需要离开减速。
     * 
     * **/
    //加速最大值
    public float accelerateMaxValueMax = 0.85f;
    public float accelerateMaxValueMin = 0.75f;
    private float accelerateMaxValue = 0.8f;
    //正常减速
    //public float normalDecelerateMaxValue = 0.3f;
    //public float normalDecelerateDampRate = 1.0f;
    //protected float normalDecelerateValue { get { return FTLibrary.Command.FTRandom.NextSingle(accelerateMinValue, normalDecelerateMaxValue); } }
    //抓住一个玩家后的减速
    public float captureDecelerateMaxValue = 0.5f;
    public float captureDecelerateDampRate = 1.5f;
    protected float captureDecelerateValue { get { return FTLibrary.Command.FTRandom.NextSingle(accelerateMinValue, captureDecelerateMaxValue); } }
    //当前需要扑捉的玩家
    public IParkourPlayerController currentCapturePlayer { get; set; }

    private PlayerAIControllerXiong m_Xiong2Ai = null;
    protected PlayerAIControllerXiong xiong2Ai
    {
        get
        {
            if (m_Xiong2Ai != null)
                return m_Xiong2Ai;
            IParkourPlayer_Xiong2 xiong2 = ((IParkourControllerScriptCS)(playerController.hControllerScriptCS)).playerXiong2;
            m_Xiong2Ai = (PlayerAIControllerXiong)xiong2.playerAIControler;
            return m_Xiong2Ai;
        }
    }

    private PlayerAIControllerXiong m_XiongDaAi = null;
    protected PlayerAIControllerXiong xiongDaAi
    {
        get
        {
            if (m_XiongDaAi != null)
                return m_XiongDaAi;
            IParkourPlayer_XiongDa xiongDa = ((IParkourControllerScriptCS)(playerController.hControllerScriptCS)).playerXiongDa;
            m_XiongDaAi = (PlayerAIControllerXiong)xiongDa.playerAIControler;
            return m_XiongDaAi;
        }
    }

    private enum GuangTouQiangStatus
    {
        Status_CapturePlayer,
        Status_SearchPlayer,
        Status_StandBy,
        Status_CaptureOKPlayer, //捕获到玩家
    }
    private GuangTouQiangStatus status = GuangTouQiangStatus.Status_SearchPlayer;
    public float waitTime = 2.0f;

    private FTLibrary.Time.TimeLocker CapturePlayerLocker = new FTLibrary.Time.TimeLocker(10000);


    protected override void Start()
    {
        base.Start();
        playerController.PlayerSystemForceAccelerateCloseEvent += OnForceAccelerateCloseEvent;
        status = GuangTouQiangStatus.Status_SearchPlayer;
        Invoke("DelaySetCapturePlayer", waitTime);
    }
    private void OnForceAccelerateCloseEvent(object sender, EventArgs e)
    {
        status = GuangTouQiangStatus.Status_SearchPlayer;
        Invoke("DelaySetCapturePlayer", waitTime);
    }
    private void DelaySetCapturePlayer()
    {
        status = GuangTouQiangStatus.Status_CapturePlayer;
        //追赶玩家
        playerController.playerAnimator.GuangtouqiangAniRandomEvent();
    }
    private IParkourPlayerController SelectOneXiong()
    {
        //首先检测次数，如果次数差值超过2就不用判断距离了
        //不需要这个了
        //if (UnityEngine.Mathf.Abs(xiong2Ai.CaptureCount-xiongDaAi.CaptureCount) >= 2)
        //{
        //    if (xiong2Ai.CaptureCount > xiongDaAi.CaptureCount)
        //    {
        //        xiongDaAi.CaptureCount += 1;
        //        return xiongDaAi.playerController;
        //    }
        //    else
        //    {
        //        xiong2Ai.CaptureCount+=1;
        //        return xiong2Ai.playerController;
        //    }
        //}
        float v = xiong2Ai.playerController.currentPlayerAccelerate - playerController.currentPlayerAccelerate;
        float v1 = xiongDaAi.playerController.currentPlayerAccelerate - playerController.currentPlayerAccelerate;
        if (v > v1)
        {
            xiongDaAi.CaptureCount += 1;
            return xiongDaAi.playerController;
        }
        else if (v < v1)
        {
            xiong2Ai.CaptureCount += 1;
            return xiong2Ai.playerController;
        }
        else
        {
            PlayerAIControllerXiong ret=FTLibrary.Command.FTRandom.Next(2) == 0 ? xiong2Ai : xiongDaAi;
            ret.CaptureCount += 1;
            return ret.playerController;
        }
    }
    public override void AIUpdate()
    {
        if (status == GuangTouQiangStatus.Status_CapturePlayer)
        {
            //检索需要扑捉的玩家
            if (currentCapturePlayer == null)
            {
                currentCapturePlayer = SelectOneXiong();
                //accelerateMaxValue = FTLibrary.Command.FTRandom.NextSingle(accelerateMaxValueMin, accelerateMaxValueMax);
                accelerateMaxValue = accelerateMaxValueMax;
                //CapturePlayerLocker.IsLocked = true;
            }
            //决策左右方向
            playerController.playerTurnDirect = playerController.PlayerLeftOrRight(currentCapturePlayer);
            //这里不在判断加速方向，直接给予目标玩家的加速度就行
            //决策加速度
            playerController.playerAccelerateSign = UnityEngine.Mathf.Clamp(currentCapturePlayer.currentPlayerAccelerate,0.0f,1.0f);
            //首先判断是否抓住这个玩家了
            //if (!CapturePlayerLocker.IsLocked)
            //{
            //    //UnityEngine.Debug.Log("lllll");
            //    //追击超过一定的值就不追了
            //    currentCapturePlayer = null;
            //    status = GuangTouQiangStatus.Status_StandBy;
            //    playerController.OpenPlayerSystemForceAccelerate(captureDecelerateValue,
            //                        captureDecelerateDampRate);
            //}
            if (playerController.currentPlayerAccelerate >= accelerateMaxValue)
            {
                //UnityEngine.Debug.Log("nnnnn");
                //追击超过一定的值就不追了
                currentCapturePlayer = null;
                status = GuangTouQiangStatus.Status_StandBy;
                playerController.OpenPlayerSystemForceAccelerate(captureDecelerateValue,
                                    captureDecelerateDampRate);
            }
            else if (playerController.IsHitThisPlayer(currentCapturePlayer,0.2f,1.6f))
            {
                OnEventMeCaptureXiong(currentCapturePlayer);
                ((PlayerAIControllerXiong)(currentCapturePlayer.playerAIControler)).OnEventGuangTouQiangCaptureMy(playerController);
                ((IParkourPlayer_Xiong)currentCapturePlayer).OnEventGuangTouQiangCaptureMy();
                //UnityEngine.Debug.Log("ccccc");
                //光头强扑捉减速
                //currentCapturePlayer = null;
                if (playerController.playerAnimator.delegateGuangtouqiangAniCollideArrestOver == null)
                {
                    playerController.playerAnimator.delegateGuangtouqiangAniCollideArrestOver = MeCaptureXiongAnimationOver;
                }
                status = GuangTouQiangStatus.Status_CaptureOKPlayer;
            }
            
        }
        else if (status == GuangTouQiangStatus.Status_SearchPlayer)
        {
            if (playerController.currentPlayerSteering > 0.1f)
            {
                playerController.playerTurnDirect = ParkourPlayerController.PlayerTurnDirect.Turn_Right;
            }
            else if (playerController.currentPlayerSteering < -0.1f)
            {
                playerController.playerTurnDirect = ParkourPlayerController.PlayerTurnDirect.Turn_Left;
            }
            else
            {
                playerController.playerTurnDirect = ParkourPlayerController.PlayerTurnDirect.Turn_Mid;
            }
        }
        else if (status == GuangTouQiangStatus.Status_CaptureOKPlayer)
        {
            //决策左右方向
            playerController.playerTurnDirect = playerController.PlayerLeftOrRight(currentCapturePlayer);
            //这里不在判断加速方向，直接给予目标玩家的加速度就行
            //决策加速度
            playerController.playerAccelerateSign = UnityEngine.Mathf.Clamp(currentCapturePlayer.currentPlayerAccelerate, 0.0f, 1.0f);
        }
        else if (status == GuangTouQiangStatus.Status_StandBy)
        {

        }
    }
    //我与障碍物发生碰撞了
    public override void  OnEventHitBrokenObject()
    {
        //UnityEngine.Debug.Log("guangtouqiang");
        status = GuangTouQiangStatus.Status_StandBy;
        playerController.OpenPlayerSystemForceAccelerate(hitDecelerateValue,
                            hitDecelerateDampRate);
        //重新寻找对象
        currentCapturePlayer = null;
        //播放碰撞动画和语言
        playerController.playerAnimator.GuangtouqiangAniCollide();

    }
    //我抓到一只熊
    public void OnEventMeCaptureXiong(IParkourPlayerController xiong)
    {
        //播放抓住动画和语言
        playerController.playerAnimator.GuangtouqiangAniCollideArrest();
    }
    //抓住熊后播完动画后回调
    public void MeCaptureXiongAnimationOver()
    {
        status = GuangTouQiangStatus.Status_StandBy;
        playerController.OpenPlayerSystemForceAccelerate(captureDecelerateValue,
                            captureDecelerateDampRate);
    }
}
