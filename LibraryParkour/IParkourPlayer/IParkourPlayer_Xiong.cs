using System;
using System.Collections.Generic;
using UnityEngine;
public class IParkourPlayer_Xiong : IParkourPlayerController
{
    //角色索引
    public enum PlayerIndex
    {
        Index_P1 = 0,
        Index_P2 = 1,
    }
    public PlayerIndex playerIndex = PlayerIndex.Index_P1;

    public override bool IsInputController
    {
        get
        {
            return base.IsInputController;
        }
        set
        {
            base.IsInputController = value;
            if (value)
            {
                InputDevice.ReadyAccountRotateSpeed((int)playerIndex);
            }
        }
    }

    //用户得分
    public int playerScore { get; set; }
    //用户最大分数上线
    public int m_PlaerMaxScore = 0;
    //是否已付费开始
    public bool IsEnterStart { get; set; }

    //氮气加速的持续时间
    public float m_SupSpeedKeepTime = 0.0f;

    //氮气加速组件
    public GameObject m_SupSpeedItem = null;
    private ParticleSystem[] m_SupSpeedParticleItem = null;

    protected override void Start()
    {
        base.Start();

        //初始化分数
        PlayerAddScore(0);
    }
    public override void PlayerControllerFixUpdate()
    {
        base.PlayerControllerFixUpdate();
        //需要需要输入控制需要对接机位
        if (IsInputController)
        {
            if (InputDevice.TurnLeft((int)playerIndex))
            {
                playerTurnDirect = PlayerTurnDirect.Turn_Left;
            }
            else if (InputDevice.TurnRight((int)playerIndex))
            {
                playerTurnDirect = PlayerTurnDirect.Turn_Right;
            }
            else
            {
                playerTurnDirect = PlayerTurnDirect.Turn_Mid;
            }
            playerAccelerateSign = playerAIControler.accelerateMinValue + InputDevice.Accelerate((int)playerIndex);

            if (InputDevice.ButtonFireDown((int)playerIndex))
            {
                if (!IsPlayerSystemForceAccelerateSign)
                {
                    StartSupSpeed();

                    Invoke("CloseSupSpeed", m_SupSpeedKeepTime);
                }
            }
        }
    }

    private void StartSupSpeed()
    {
        if (m_SupSpeedParticleItem == null)
        {
            m_SupSpeedParticleItem = m_SupSpeedItem.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < m_SupSpeedParticleItem.Length; i++)
            {
                m_SupSpeedParticleItem[i].Stop();
                m_SupSpeedParticleItem[i].Play();
            }
        }
        else
        {
            for (int i = 0; i < m_SupSpeedParticleItem.Length; i++)
            {
                m_SupSpeedParticleItem[i].Stop();
                m_SupSpeedParticleItem[i].Play();
            }
        }

        //IsPlayerSystemForceAccelerateSign = true;
        m_PlayerAccelerateSign = 1.0f;
        OpenPlayerSystemForceAccelerate(m_PlayerAccelerateSign, m_PlayerAccelerateSign);
        m_SupSpeedItem.SetActive(true);
    }

    private void CloseSupSpeed()
    {
        //IsPlayerSystemForceAccelerateSign = false;
        m_SupSpeedItem.SetActive(false);
        ClosePlayerSystemForceAccelerate();
    }

    //喷射的粒子列表
    //public ParticleSystem[] m_particleList = null;
    //创建粒子的参照点
    public GameObject m_ParticlePoint = null;

    protected override void OnTriggerEnter(Collider other)
    {
        BrokenObjectController broken = other.gameObject.GetComponent<BrokenObjectController>();
        if (broken == null)
        {
            return;
        }

        if (broken.IsLocker)
        {
            return;
        }

        broken.IsLocker = true;
        //m_particleList = broken.particleList;
        //if (m_particleList != null && m_particleList.Length != 0)
        //{
        //    PlayParticleList(m_particleList);
        //}
        if (broken.m_PropParticleStruct != null)
        {
            PlayParticleList(broken.m_PropParticleStruct);
        }

        //处理被撞物品
        broken.StartBroken(hControllerScriptCS as IParkourControllerScriptCS);
        if (broken.brokenHandleType == BrokenObjectController.BrokenHandleType.HandleType_Barrier)
        {
            //触发碰撞消息
            playerAIControler.OnEventHitBrokenObject();
            //播放碰撞语言
            SoundEffectPlayer.Play("touchbroken.wav");
            //playerSoundControler.PlayAudioSource(IParkourPlayerSound.PlayerSoundType.Type_Barrier);
        }
        else if (broken.brokenHandleType == BrokenObjectController.BrokenHandleType.HandleType_AddScore)
        {
            ////如果没有开始游戏 则不加分 跳过
            //if (!IsEnterStart)
            //{
                ////播放加分语言
                //playerSoundControler.PlayAudioSource(IParkourPlayerSound.PlayerSoundType.Type_AddScore);
                //return;
            //}
            //给玩家加分
            PlayerAddScore((int)broken.brokenValue);
            //播放加分语言
            SoundEffectPlayer.Play("touchgold.wav");
            //playerSoundControler.PlayAudioSource(IParkourPlayerSound.PlayerSoundType.Type_AddScore);
        }
    }

    //吃道具 粒子喷射
    private void PlayParticleList(ParticleSystem[] particlelist)
    {
        for (int i = 0; i < particlelist.Length; i++)
        {
            //particlelist[i].gameObject.transform.parent = m_ParticlePoint.transform;
            particlelist[i].gameObject.transform.position = m_ParticlePoint.transform.position;
            //喷射粒子
            particlelist[i].enableEmission = true;
            particlelist[i].Stop();
            particlelist[i].Play();
        }
    }
    private void PlayParticleList(SenceObjectModControl.PropParticleStruct[] propParticleStruct)
    {
        
        for (int i = 0; i < propParticleStruct.Length; i++)
        {
            GameObject psobject = GameRoot.gameResource.LoadResource_Prefabs(propParticleStruct[i].m_PropParticlePrefabName);
            psobject.transform.parent = m_ParticlePoint.transform;
            psobject.transform.position = m_ParticlePoint.transform.position;
            ParticleSystem ps = psobject.GetComponent<ParticleSystem>();
            //喷射粒子
            ps.enableEmission = true;
            ps.Stop();
            ps.Play();
        }
        //Prop.particleList = new ParticleSystem[boc.m_PropParticleStruct.Length];
        //ParticleSystem ps = null;
        //for (int i = 0; i < Prop.particleList.Length; i++)
        //{
        //    GameObject psobject = GameRoot.gameResource.LoadResource_Prefabs(boc.m_PropParticleStruct[i].m_PropParticlePrefabName);
        //    Transform pstransform = psobject.transform;
        //    ps = psobject.GetComponent<ParticleSystem>();
        //    Prop.particleList[i] = ps;
        //    pstransform.parent = Prop.gameObject.transform;
        //    pstransform.localPosition = new Vector3(Vector3.zero.x, m_setParticleLocalHight, Vector3.zero.z);

        //}
    }

    //用户加分，用于刷新界面，播放界面文字等
    private void PlayerAddScore(int score)
    {
        //只有游戏过程中的分才计算到奖品计算中
        if (GameRoot.gameProcessControl.currentProcess.processType == ModalProcessType.Process_GameRace &&
            IsInputController)
        {
            InputDevice.PlayerAddScores((int)playerIndex, score);
        }
        
        if (playerScore < 0)
        {
            playerScore = 0;
        }
        else if (playerScore > m_PlaerMaxScore)
        {
            playerScore = m_PlaerMaxScore;
        }
        else
        {
            playerScore += score;
        }
        ((RaceSceneControl)GameRoot.CurrentSceneControl).GetPlayerScore();
        ((RaceSceneControl)GameRoot.CurrentSceneControl).m_PlayerScoreControl.GetScore();
    }

    //被抓住的提示界面
    private GameObject Prompt_Accelerate = null;
    public bool IsShowPrompt_Accelerate { get; set; }
    //光头强抓到我了
    public void OnEventGuangTouQiangCaptureMy()
    {
        if (!IsEnterStart || Prompt_Accelerate != null)
            return;
        if (((IParkourControllerScriptCS)hControllerScriptCS).isGamePaused)
            return;
        if (!IsShowPrompt_Accelerate)
            return;
        Release_Prompt_Accelerate();
        switch (playerIndex)
        {
            case PlayerIndex.Index_P1:
                Prompt_Accelerate = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_Accelerate_Left.prefab", GameRoot.gameResource);
                break;
            case PlayerIndex.Index_P2:
                Prompt_Accelerate = GameRoot.uiOrthographicCamera.LoadLanguageResource_UIPrefabs("Prompt_Accelerate_Right.prefab", GameRoot.gameResource);
                break;
        }
        Invoke("Release_Prompt_Accelerate", 3.0f);
        SoundEffectPlayer.PlayStandalone("Accelerate.wav");
    }
    public void Release_Prompt_Accelerate()
    {
        if (Prompt_Accelerate != null)
        {
            UnityEngine.Object.DestroyObject(Prompt_Accelerate);
            Prompt_Accelerate = null;
        }
        
    }
}
