using System;
using System.Collections.Generic;
using UnityEngine;
public class BrokenObjectController : MonoBehaviourIgnoreGui
{
    //主体对象
    public Transform mainObject = null;
    //喷射的粒子列表
    public ParticleSystem[] particleList = null;
    [HideInInspector]
    public SenceObjectModControl.PropParticleStruct[] m_PropParticleStruct = null;


    //存留的时间
    public float remainTime = 1.0f;
    //protected bool IsParticleEmission
    //{
    //    get 
    //    {
    //        if (particleList == null || particleList.Length == 0)
    //            return false;
    //        return particleList[0].enableEmission;
    //    }
    //    set
    //    {
    //        if (particleList == null || particleList.Length == 0)
    //            return;
    //        for (int i = 0; i < particleList.Length;i++ )
    //        {
    //            particleList[i].enableEmission = value;
    //        }
    //    }
    //}

    //类型
    public enum BrokenType
    {
        BrokenType_Beehive,     //蜂窝
        BrokenType_Fish,        //鱼
        BrokenType_Pinecone,    //送塔
        BrokenType_Banana,      //香蕉
        BrokenType_Apple,       //苹果
        BrokenType_Chips,       //薯片
        BrokenType_Watermelon,  //西瓜
        BrokenType_Fence,       //栅栏
        BrokenType_Haystack,    //草垛
        BrokenType_Scarecrow,   //稻草人
        BrokenType_JaggedWood,  //断木
        BrokenType_Stub,        //树桩
    }
    public enum BrokenHandleType
    {
        HandleType_AddScore,    //加分
        HandleType_Barrier,     //障碍物
    }
    public BrokenType brokenType = BrokenType.BrokenType_Apple;
    public BrokenHandleType brokenHandleType = BrokenHandleType.HandleType_AddScore;
    public float brokenValue = 0.0f;

    //被锁定
    public bool IsLocker { get; set; }
    protected virtual void Start()
    {
        //IsParticleEmission = false;
        IsLocker = false;
        if (mainObject == null)
        {
            mainObject = gameObject.transform;
        }
    }
    public virtual void StartBroken(IParkourControllerScriptCS iParkourControllerScriptCS)
    {
        //将主体隐藏
        mainObject.gameObject.renderer.enabled = false;//.SetActive(false);
        ////喷射粒子
        //IsParticleEmission = true;
        //if (particleList != null && particleList.Length != 0)
        //{
        //    for (int i = 0; i < particleList.Length; i++)
        //    {
        //        particleList[i].Stop();
        //        particleList[i].Play();
        //    }
        //}

        ////播放声音
        //if (AudioClipNameList != null && AudioClipNameList.Length != 0)
        //{
        //    for (int i = 0; i < AudioClipNameList.Length;i++ )
        //    {
        //        iParkourControllerScriptCS.PlayBrokenSound(AudioClipNameList[i], transform.position);
        //    }
        //}
        //延迟删除自己
        Invoke("DelayDestroy", remainTime);
    }
    private void DelayDestroy()
    {
        GameObject.DestroyObject(gameObject);
    }
}
