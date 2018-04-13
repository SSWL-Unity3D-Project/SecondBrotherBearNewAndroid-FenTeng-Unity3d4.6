using UnityEngine;
using System.Collections;
using FTLibrary.Time;
using System.Collections.Generic;
using FTLibrary.XML;
using System;

//玩家角色控制
public class PlayerAnimationControl : MonoBehaviourIgnoreGui
{
    internal enum PlayerAniType
    {
        G_PlayerRun = 0,        //光头强骑车
        G_PlayerFire,           //光头强射击动画
        G_PlayerCollide,        //光头强碰撞道具
        G_PlayerArrestBear,     //光头强抓捕熊
        X_PlayerRun,            //熊骑车
        X_PlayerLookLeft,       //熊向左看
        X_PlayerLookRight,      //熊向左看
        X_PlayerCollide,        //熊碰撞道具
        X_PlayerSprint,         //熊加速
        X_Hurt,
    }

    //玩家类型
    internal enum PlayerType
    {
        PlayerType_Guangtouqiang,
        PlayerType_BrotherBear,
    }

    internal PlayerAniType m_PlayerAniType;

    //玩家控制
    internal IParkourPlayerController m_PlayerController = null;

    /// <summary>
    /// 熊运动控制脚本.
    /// </summary>
    public IParkourPlayer_Xiong m_IParkourPlayer_Xiong;
    
    //玩家动画控制
    public Animation m_AnimationControl;

    //随机事件参数
    //熊的随机事件参数
    public float m_RandomEventTimer_BearBegin = 3.0f;
    public float m_RandomEventTimer_BearEnd = 8.0f;
    public float m_RandomEventTimer_Bear = 0.0f;
    public int m_RandomEventParam_Bear = 2;
    //光头强的随机事件参数
    public float m_RandomEventTimer_Guangtouqiang = 3.0f;
    public int m_RandomEventParam_Guangtouqiang = 3;

    //当前时间
    private float m_NowTimer = 0.0f;

    //玩家准备播放的动画类型
    internal PlayerAniType m_ReadyPlayerAniType;

    //动画参数列表
    internal class AnimationParam
    {
        internal int id;
        internal string AnimationName;
        internal string paramName;
        internal float paramValue;
        internal float nowTimer;
        internal float delValue;
        internal float delTimer;
    }
    //动画参数列表
    internal Dictionary<int, AnimationParam> m_AnimationParamlist = new Dictionary<int, AnimationParam>(5);
    internal AnimationParam GetAnimationParam(int id)
    {
        AnimationParam obj = null;
        if (m_AnimationParamlist.TryGetValue(id, out obj))
        {
            return obj;
        }
        else
        {
            return null;
        }
    }
    ////动画当前使用使用列表
    internal List<AnimationParam> m_NowUseAnimationParamlist = new List<AnimationParam>(5);

    internal void Initialization(IParkourPlayerController PlayerController)
    {
        m_PlayerController = PlayerController;
        m_RandomEventTimer_Bear = UnityEngine.Random.Range(m_RandomEventTimer_BearBegin, m_RandomEventTimer_BearEnd);
        /*
         * 重点问题：
         * 加密狗在安卓上几个小时后就会休眠，所有和加密狗的访问都会发生崩溃。
         * 所以，在整个游戏运用过程中不应该在访问加密狗已经访问和加密狗相关联的任何功能了。
         * 这里使用缓冲机制处理xml的读取
         * 
         * */
         //XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile("HumanoidAnimation.xml");
        XmlDocument doc = GameRoot.gameResource.BuffResource_XmlFile("HumanoidAnimation.xml");
         XmlNodeList nodelist = doc.SelectNodes("item");
        int i = 0;

        for (i = 0; i < nodelist.Count; i++)
        {
            AnimationParam obj = new AnimationParam();
            obj.id = Convert.ToInt32(nodelist[i].Attribute("AniID"));
            obj.AnimationName = Convert.ToString(nodelist[i].Attribute("AnimationName"));
            obj.paramName = Convert.ToString(nodelist[i].Attribute("paramName"));
            obj.paramValue = (float)Convert.ToDouble(nodelist[i].Attribute("paramValue"));
            obj.nowTimer = 0.0f;
            obj.delValue = (float)Convert.ToDouble(nodelist[i].Attribute("delValue"));
            obj.delTimer = (float)Convert.ToDouble(nodelist[i].Attribute("delTimer"));
            m_AnimationParamlist.Add(obj.id, obj);
        }
    }

    public void Update()
    {
        m_NowTimer += Time.deltaTime;
        if(m_PlayerController != null)
        {
            if (m_PlayerController.playerType == IParkourPlayerController.PlayerType.Type_GuangTouQiang)
            {
                GuangtouqiangUpdate();
            }
            else if (m_PlayerController.playerType == IParkourPlayerController.PlayerType.Type_Xiong2 ||
                m_PlayerController.playerType == IParkourPlayerController.PlayerType.Type_XiongDa)
            {
                BrotherBearUpdate();
            }
        }

        if (m_NowUseAnimationParamlist.Count != 0)
        {
            for (int i = 0; i < m_NowUseAnimationParamlist.Count; i++)
            {
                AnimationParam obj = m_NowUseAnimationParamlist[i];
                obj.nowTimer += Time.deltaTime;
                if (obj.nowTimer > obj.delTimer)
                {
                    if (obj.id == (int)PlayerAniType.G_PlayerArrestBear && delegateGuangtouqiangAniCollideArrestOver != null)
                    {
                        delegateGuangtouqiangAniCollideArrestOver();
                    }
                    m_NowUseAnimationParamlist.RemoveAt(i);
                    i++;
                }
            }
        }
    }

    //光头强的刷新函数
    private void GuangtouqiangUpdate()
    {
        if (!m_AnimationControl.isPlaying)
        {
            AnimationParam obj = GetAnimationParam((int)PlayerAniType.G_PlayerRun);
            m_AnimationControl.wrapMode = WrapMode.Loop;
            m_AnimationControl.CrossFade(obj.AnimationName);
        }
    }

    //光头强的动画触发函数
    public void GuangtouqiangAniRandomEvent()
    {
        int EventParam = UnityEngine.Random.Range(0, m_RandomEventParam_Guangtouqiang);
        if (EventParam <= 1)
        {
            //Debug.Log("光头强开火！！！！！！！！！！！！！！1");
            AnimationParam obj = GetAnimationParam((int)PlayerAniType.G_PlayerFire);
            if (obj == null)
            {
                Debug.Log("没找对动画参数");
            }
            m_AnimationControl.wrapMode = WrapMode.Once;
            m_AnimationControl.CrossFade(obj.AnimationName);
            //obj.nowTimer = 0.0f;
            //m_NowUseAnimationParamlist.Add(obj);
        }
    }

    //光头强的被撞动画
    public void GuangtouqiangAniCollide()
    {
        AnimationParam obj = GetAnimationParam((int)PlayerAniType.G_PlayerCollide);
        if (obj == null)
        {
            Debug.Log("没找对动画参数");
        }
        m_AnimationControl.wrapMode = WrapMode.Once;
        m_AnimationControl.CrossFade(obj.AnimationName);
        //obj.nowTimer = 0.0f;
        //m_NowUseAnimationParamlist.Add(obj);
    }

    //光头钱抓到狗熊后的回调
    public delegate void DelegateGuangtouqiangAniCollideArrestOver();
    public DelegateGuangtouqiangAniCollideArrestOver delegateGuangtouqiangAniCollideArrestOver = null;
    //光头强抓捕狗熊的动画
    public void GuangtouqiangAniCollideArrest()
    {
        AnimationParam obj = GetAnimationParam((int)PlayerAniType.G_PlayerArrestBear);
        if (obj == null)
        {
            Debug.Log("没找对动画参数");
        }
        m_AnimationControl.wrapMode = WrapMode.Once;
        m_AnimationControl.CrossFade(obj.AnimationName);
        obj.nowTimer = 0.0f;
        m_NowUseAnimationParamlist.Add(obj);
    }

    //狗熊的刷新函数
    private void BrotherBearUpdate()
    {
        if (m_NowTimer > m_RandomEventTimer_Bear)
        {
            m_NowTimer = 0.0f;
            m_RandomEventTimer_Bear = UnityEngine.Random.Range(m_RandomEventTimer_BearBegin, m_RandomEventTimer_BearEnd);
            int EventParam = UnityEngine.Random.Range(0, m_RandomEventParam_Bear);
            AnimationParam obj = null;
             if (EventParam == 0)
             {
                 obj = GetAnimationParam((int)PlayerAniType.X_PlayerLookLeft);
                
             }
             else if (EventParam == 1)
             {
                 obj = GetAnimationParam((int)PlayerAniType.X_PlayerLookRight);
             }
             else
             {
                 return;
             }
             if (obj == null)
             {
                 Debug.Log("没找对动画参数");
             }
             m_AnimationControl.wrapMode = WrapMode.Once;
             m_AnimationControl.CrossFade(obj.AnimationName);
             //obj.nowTimer = 0.0f;
             //m_NowUseAnimationParamlist.Add(obj);
        }

       if (!m_AnimationControl.isPlaying)
       {
           AnimationParam obj = GetAnimationParam((int)PlayerAniType.X_PlayerRun);
            m_AnimationControl.wrapMode = WrapMode.Loop;
            m_AnimationControl.CrossFade(obj.AnimationName);
       }

       if (m_IParkourPlayer_Xiong != null && m_IParkourPlayer_Xiong.IsInputController)
       {
           float accVal = InputDevice.Accelerate((int)m_IParkourPlayer_Xiong.playerIndex);
            m_AnimationControl[PlayerAniType.X_PlayerRun.ToString()].speed = 1f + accVal;
       }
    }

    //狗熊撞到障碍物动画
    public void BrotherBearAniCollide()
    {
        //Debug.Log("狗熊碰墙");
        AnimationParam obj = GetAnimationParam((int)PlayerAniType.X_PlayerCollide);
        if (obj == null)
        {
            Debug.Log("没找对动画参数");
        }
        m_AnimationControl.wrapMode = WrapMode.Once;
        m_AnimationControl.CrossFade(obj.AnimationName);
        //obj.nowTimer = 0.0f;
        //m_NowUseAnimationParamlist.Add(obj);
    }

    //狗熊被光头强抓着的动画
    public void BrotherBearAniCollideArrest()
    {
       // Debug.Log("狗熊被抓");
        //AnimationParam obj = GetAnimationParam((int)PlayerAniType.X_PlayerSprint);
        AnimationParam obj = GetAnimationParam((int)PlayerAniType.X_Hurt);
        if (obj == null)
        {
            Debug.Log("没找对动画参数");
        }
        m_AnimationControl.wrapMode = WrapMode.Once;
        m_AnimationControl.CrossFade(obj.AnimationName);


        obj = GetAnimationParam((int)PlayerAniType.X_PlayerSprint);
        if (obj == null)
        {
            Debug.Log("没找对动画参数");
        }
        m_AnimationControl.wrapMode = WrapMode.Once;
        m_AnimationControl.CrossFadeQueued(obj.AnimationName);
        //obj.nowTimer = 0.0f;
        //m_NowUseAnimationParamlist.Add(obj);
    }
}
