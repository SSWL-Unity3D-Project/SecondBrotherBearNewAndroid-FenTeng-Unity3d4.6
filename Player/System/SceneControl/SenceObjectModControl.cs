using UnityEngine;
using System.Collections;
using FTLibrary.XML;
using System;

/// <summary>
/// 
/// 场景模版控制 需要从模版上获取东西 都在这获取
/// 
/// </summary>

public class SenceObjectModControl : MonoBehaviourIgnoreGui
{
    //道具模版结构
    struct PropModStruct
    {
        public string m_PropName;
        public string m_PropPrefabName;
        public BrokenObjectController m_BrokenObjectController;
        public LayerMask m_PropLayer;
        public PropParticleStruct[] m_PropParticleStruct;
        public PropSoundStruct[] m_PropSoundStruct;
    }

    //道具粒子特效模版结构
    public struct PropParticleStruct
    {
        public string m_PropParticleName;
        public string m_PropParticlePrefabName;
        public PropParticleState m_PropParticleState;
    }
    //道具粒子特效模版类型
    public enum PropParticleState
    {
        always = 0,
        AddScore = 1,
        Barrier = 2
    }

    //道具音效模版结构
    struct PropSoundStruct
    {
        public string m_PropSoundName;
        public PropSoundState m_PropSoundState;
    }
    //道具音效模版类型
    enum PropSoundState
    {
        AddScore = 0,
        Barrier = 1
    }

    //道具模版的父对象
    private GameObject m_PropsParent = null;
    private Transform m_PropsParentTransform;

    //道具模版组
    //internal BrokenObjectController[] m_BrokenObjectControllerItem = new BrokenObjectController[2];
    //道具模版结构组
    private PropModStruct[] m_propModStructItem = new PropModStruct[2];

    //道具粒子模版对象组
    internal GameObject[] m_PropParticleObjectItem = new GameObject[2];
    //道具粒子模版结构组
    private PropParticleStruct[] m_PropParticleStructItem = new PropParticleStruct[2];

    //道具音效模版对象组
    internal AudioSource[] m_PropSoundObjectItem = new AudioSource[2];
    //道具音效模版结构组
    private PropSoundStruct[] m_PropSoundStructItem = new PropSoundStruct[2];

    public void Initialization()
    {
        m_PropsParent = new GameObject("PropsParent");
        m_PropsParentTransform = m_PropsParent.transform;
        m_PropsParentTransform.parent = this.gameObject.transform;
        LoadXML();
    }

    //加载xml中的文件
    private void LoadXML()
    {
        //加载xml
        XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile("SenceMod.xml");
        if (doc == null)
        {
            throw new Exception("SenceMod.xml is null");
        }

        //获取根节点
        XmlNode root = doc.SelectSingleNode("sencemodconfig");


        //获取道具粒子模版节点
        XmlNode PropParticleItemNode = root.SelectSingleNode("propmodparticle");
        //获取道具粒子模版
        XmlNodeList PropParticleList = PropParticleItemNode.SelectNodes("propparticle");
        //读取存储
        m_PropParticleStructItem = new PropParticleStruct[PropParticleList.Count];
        for (int i = 0; i < m_PropParticleStructItem.Length; i++)
        {
            XmlNode PropParticleNode = PropParticleList[i];
            m_PropParticleStructItem[i].m_PropParticleName = Convert.ToString(PropParticleNode.Attribute("name"));
            //Debug.Log("m_PropParticleStructItem[i].m_PropParticleName::" + m_PropParticleStructItem[i].m_PropParticleName);
            m_PropParticleStructItem[i].m_PropParticlePrefabName = Convert.ToString(PropParticleNode.Attribute("prefabname"));
            //Debug.Log("m_PropParticleStructItem[i].m_PropParticlePrefabName::"+m_PropParticleStructItem[i].m_PropParticlePrefabName);
            m_PropParticleStructItem[i].m_PropParticleState = (PropParticleState)Convert.ToInt32(PropParticleNode.Attribute("state"));
            //Debug.Log("m_PropParticleStructItem[i].m_PropParticleState::" + m_PropParticleStructItem[i].m_PropParticleState);

        }


        //获取道具音效模版节点
        XmlNode PropSoundItemNode = root.SelectSingleNode("propmodsound");
        //获取道具音效模版
        XmlNodeList PropSoundList = PropSoundItemNode.SelectNodes("propsound");
        //读取存储
        m_PropSoundStructItem = new PropSoundStruct[PropSoundList.Count];
        for (int i = 0; i < m_PropSoundStructItem.Length; i++)
        {
            XmlNode PropSoundNode = PropSoundList[i];
            m_PropSoundStructItem[i].m_PropSoundName = Convert.ToString(PropSoundNode.Attribute("name"));
            m_PropSoundStructItem[i].m_PropSoundState = (PropSoundState)Convert.ToInt32(PropSoundNode.Attribute("state"));
        }


        //获取道具模版组节点
        XmlNode PropItemNode = root.SelectSingleNode("propmod");
        //获取道具模版
        XmlNodeList PropList = PropItemNode.SelectNodes("prop");
        //读取存储
        //m_BrokenObjectControllerItem = new BrokenObjectController[PropList.Count];
        m_propModStructItem = new PropModStruct[PropList.Count];
        for (int i = 0; i < m_propModStructItem.Length; i++)
        {
            XmlNode PropNode = PropList[i];
            m_propModStructItem[i] = new PropModStruct();
            m_propModStructItem[i].m_PropName = Convert.ToString(PropNode.Attribute("name"));
            m_propModStructItem[i].m_PropPrefabName = Convert.ToString(PropNode.Attribute("prefabname"));

            //实例化模版
            GameObject propModObject = GameRoot.gameResource.LoadResource_Prefabs(m_propModStructItem[i].m_PropPrefabName);
            propModObject.transform.parent = m_PropsParentTransform;
            m_propModStructItem[i].m_BrokenObjectController = propModObject.GetComponent<BrokenObjectController>();
            m_propModStructItem[i].m_PropLayer = propModObject.layer;

            //Debug.Log("m_propModStructItem[i].m_BrokenObjectController:::" + m_propModStructItem[i].m_BrokenObjectController);

            if (m_propModStructItem[i].m_BrokenObjectController == null)
            {
                throw new Exception(propModObject.name + "道具脚本错误");
            }

            //获取组件内的粒子特效参数 存相应的结构
            if (m_propModStructItem[i].m_BrokenObjectController.particleList != null)
            {
                m_propModStructItem[i].m_PropParticleStruct = new PropParticleStruct[m_propModStructItem[i].m_BrokenObjectController.particleList.Length];
                for (int j = 0; j < m_propModStructItem[i].m_PropParticleStruct.Length; j++)
                {
                    m_propModStructItem[i].m_PropParticleStruct[j] = GetPropParticleStruct(m_propModStructItem[i].m_BrokenObjectController.particleList[j]);
                }
            }

            //获取组件内的音频的参数 存相应的结构
            //if (m_propModStructItem[i].m_BrokenObjectController.AudioClipNameList != null)
            //{
            //    m_propModStructItem[i].m_PropSoundStruct = new PropSoundStruct[m_propModStructItem[i].m_BrokenObjectController.AudioClipNameList.Length];
            //    for (int j = 0; j < m_propModStructItem[i].m_PropSoundStruct.Length; j++)
            //    {
            //        m_propModStructItem[i].m_PropSoundStruct[j] = GetPropSoundStruct(m_propModStructItem[i].m_BrokenObjectController.AudioClipNameList[j]);
            //    }
            //}

            //读取完信息 关掉
            propModObject.SetActive(false);
        }

    }

    //获取粒子特效模版结构
    private PropParticleStruct GetPropParticleStruct(ParticleSystem particle)
    {
        string particleName = particle.gameObject.name;

        for (int i = 0; i < m_PropParticleStructItem.Length; i++)
        {
            if (particleName.Equals(m_PropParticleStructItem[i].m_PropParticleName))
            {
                return m_PropParticleStructItem[i];
            }
        }

        return m_PropParticleStructItem[0];
    }

    //获取音频模版结构
    private PropSoundStruct GetPropSoundStruct(string sound)
    {
        for (int i = 0; i < m_PropSoundStructItem.Length; i++)
        {
            if (sound.Equals(m_PropSoundStructItem[i].m_PropSoundName))
            {
                return m_PropSoundStructItem[i];
            }
        }
        return m_PropSoundStructItem[0];
    }

    //工厂 包装道具组
    public void AddPropModItem(BrokenObjectController[] PropItem)
    {
        for (int i = 0; i < PropItem.Length; i++)
        {
            AddPropMod(PropItem[i]);
        }
    }

    //粒子特效的相对高度
    public float m_setParticleLocalHight = 0.0f;

    //工厂 包装道具
    public void AddPropMod(BrokenObjectController Prop)
    {
        PropModStruct boc = new PropModStruct();
        for (int i = 0; i < m_propModStructItem.Length; i++)
        {
            if (Prop.brokenType == m_propModStructItem[i].m_BrokenObjectController.brokenType)
            {
                boc = m_propModStructItem[i];
                break;
            }
        }

        if (boc.m_BrokenObjectController == null)
        {
            throw new Exception("没有匹配到路面Prop");
        }

        //复制碰撞层
        Prop.gameObject.layer = boc.m_PropLayer;

        //复制物品的类型
        Prop.brokenHandleType = boc.m_BrokenObjectController.brokenHandleType;
        //复制物品的分数
        Prop.brokenValue = boc.m_BrokenObjectController.brokenValue;
        Prop.remainTime = boc.m_BrokenObjectController.remainTime;

        //音频复制
        //Prop.AudioClipNameList = boc.m_BrokenObjectController.AudioClipNameList;

        //存放粒子特效
        Prop.m_PropParticleStruct = boc.m_PropParticleStruct;
        
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

    //生成模版对象
    private void IniObjectMod()
    {

    }

}
