using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
class SceneControlRoot : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Root; } }
    //这种方式是
    //改成Start初始化，不然Awake 不能初始化系统Time
    protected override void Start()
    {
        BootingSystem();
    }
    //打开游戏启动画面
    protected virtual void OpenGameBootFace()
    {
        UnityEngine.GameObject obj = GameObject.Instantiate(Resources.Load(UniGameResourcesDefine.GameBoot_Resource_Path,
                            typeof(GameObject))) as UnityEngine.GameObject;

        SceneControl.gameBootFace = obj.GetComponent<UniGameBootFace>();
    }
    protected virtual void BootingSystem()
    {
        //显示启动页面
        OpenGameBootFace();
        Invoke("BootingSystemOver", 0.01f);
    }
    private void BootingSystemOver()
    {
        //这里GameRoot的初始化在GameRoot的静态构造函数里
        //这里需要检测GameRoot是否加载完成了
        GameRoot.GameRootInitializationSucceedEvent += OnGameRootInitializationSucceed;
        GameRoot.StartInitialization();
    }
    public void OnGameRootInitializationSucceed(object sender, EventArgs e)
    {
        //资源已经全部下载完成了
        //开始进行下面的过程
        base.Start();
    }
    protected override void LinkSceneInfo()
    {
        
    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        base.Initialization();
        if (SceneControl.gameBootFace == null)
        {
            //开始自检过程
            GameRoot.gameProcessControl.ActivateProcess(typeof(SelfCheckingModalProcess));
        }
        else
        {
            SceneControl.gameBootFace.bootFacePlayer.DelegateOnPlayEndEvent = GameBootFacePlayEnd;
            SceneControl.gameBootFace.bootFacePlayer.Play();
        }
    }
    private void GameBootFacePlayEnd()
    {
        SceneControl.gameBootFace.bootFacePlayer.DelegateOnPlayEndEvent = null;
        UnityEngine.Object.DestroyObject(SceneControl.gameBootFace.gameObject);
        SceneControl.gameBootFace = null;
        //开始自检过程
        GameRoot.gameProcessControl.ActivateProcess(typeof(SelfCheckingModalProcess));
    }
    protected override void OnDestroyScene()
    {
        base.OnDestroyScene();
    }
}
