using System;
using System.Collections.Generic;
using UnityEngine;

public class IParkourPlayerController : ParkourPlayerController
{
    public enum PlayerType
    {
        Type_XiongDa,
        Type_Xiong2,
        Type_GuangTouQiang,
        Type_Camera,
    }
    public virtual PlayerType playerType { get { return PlayerType.Type_Camera; } }
    public PlayerAnimationControl playerAnimator;
    //车轮粒子
    public ParticleSystem[] particleList = null;
    //是否喷射粒子
    private bool m_IsParticleEmission = false;
    protected bool IsParticleEmission
    {
        get { return m_IsParticleEmission; }
        set
        {
            if (m_IsParticleEmission == value)
                return;
            m_IsParticleEmission = value;
            if (particleList == null || particleList.Length == 0)
                return;
            for (int i = 0; i < particleList.Length; i++)
            {
                particleList[i].enableEmission = m_IsParticleEmission;
            }
        }
    }
    //是否AI控制
    public bool IsAIController { get; set; }
    //是否可以输入控制
    private bool isinputcontroller = false;
    public virtual bool IsInputController 
    {
        get
        {
            return isinputcontroller;
        }
        set
        {
            isinputcontroller = value;
        }
    }
    //角色的AI控制
    public PlayerAIController playerAIControler = null;
    //角色的声音控制
    public IParkourPlayerSound playerSoundControler = null;
    protected override void Start()
    {
        base.Start();
        playerAnimator = (PlayerAnimationControl)GetComponent(typeof(PlayerAnimationControl));
        if (playerAnimator != null)
        {
            playerAnimator.Initialization(this);
        }
    }
    public override void PlayerControllerFixUpdate()
    {
        if (IsAIController && playerAIControler != null)
            playerAIControler.AIUpdate();
        base.PlayerControllerFixUpdate();
        if (IsParticleEmission != IsGroundhitRoad)
        {
            IsParticleEmission = IsGroundhitRoad;
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }
}
