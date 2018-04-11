using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
class SceneControl : MonoBehaviourIgnoreGui
{
    //启动画面对象
    public static UniGameBootFace gameBootFace = null;
    public virtual SceneControlType SceneType { get { return SceneControlType.SceneControl_Base; } }
    //改成Start初始化，不然Awake 不能初始化系统Time
    protected virtual void Start()
    {
        GameRoot.StartInitialization();
        GameRoot.RegisterScene(this);
    }
    protected virtual void OnDestroy()
    {
        OnDestroyScene();
        GameRoot.UnRegisterScene(this);
    }
    protected virtual void LinkSceneInfo()
    {
       
    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public virtual void Initialization()
    {
        LinkSceneInfo();
    }
    protected virtual void OnDestroyScene()
    {

    }
}
