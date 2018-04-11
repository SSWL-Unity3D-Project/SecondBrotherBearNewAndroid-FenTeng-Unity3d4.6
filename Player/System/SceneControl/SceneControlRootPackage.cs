using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
class SceneControlRootPackage : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Root; } }

    internal enum BootingSystemStep
    {
        Step_Nothing =0,
        Step_ConstraintInstallPackage,      //下载并加载必需的资源
        Step_ConstraintDownloadPackage,     //下载必需更新的资源
        Step_NeedInstallNewVersionProgram,  //需要安装新版本的程序
        Step_BootingOver,                   //启动完成
    }
    public BootingSystemStep bootingSystemStep { get; set; }
    //必要装载的资源包
    protected UniGameResourcesDownLoader constraintInstallDownloader = null;
    //必需下载的资源包
    protected UniGameResourcesDownLoader constraintDownloadDownloader = null;
    //进度值
    public float Progress
    {
        get
        {
            switch(bootingSystemStep)
            {
                case BootingSystemStep.Step_Nothing:
                    return 0.0f;
                case BootingSystemStep.Step_ConstraintInstallPackage:
                    return constraintInstallDownloader == null ? 0.0f : constraintInstallDownloader.Progress;
                case BootingSystemStep.Step_ConstraintDownloadPackage:
                    return constraintDownloadDownloader == null ? 0.0f : constraintDownloadDownloader.Progress;
                default:
                    return 1.0f;
            }
        }
    }
    //这种方式是
    //改成Start初始化，不然Awake 不能初始化系统Time
    protected override void Start()
    {
        //首先需要激活系统，否则在载入资源清单的时候，文件就无法解密了
        GameRoot.Activation_Environment();
        StartCoroutine(BootingSystem());
    }
    protected override void OnDestroyScene()
    {
        base.OnDestroyScene();
    }
    //打开游戏启动画面
    protected virtual IEnumerator OpenGameBootFace()
    {
        //检测是否有系统启动界面
        if (UniGameResources.FindSystemResourcesPackageTable(UniGameResourcesDefine.DefineAssetBundleName_GameBoot) == null)
            yield break;
        uint guid = UniGameResources.resourcesDownLoadWorker.AllocLoadingAssetObjectGUID();
        yield return StartCoroutine(UniGameResources.resourcesDownLoadWorker.LoadingAssetObject(UniGameResourcesDefine.DefineAssetBundleName_GameBoot,
                            UniGameResourcesDefine.GameBoot_AssetBundle_Path, guid, typeof(UnityEngine.GameObject), true));
        UnityEngine.GameObject obj = UniGameResources.resourcesDownLoadWorker.PutoffAssetObject(guid) as UnityEngine.GameObject;
        SceneControl.gameBootFace = obj.GetComponent<UniGameBootFace>();
    }

    protected virtual IEnumerator BootingSystem()
    {
        //重新设置资源加载模式
        UniGameResources.gameResourcesWorkMode = UniGameResources.GameResourcesWorkMode.Mode_Mobile;
        //打开设备文件加密
        UniGameResources.IsSupportDeviceFileEncrypt = true;
        //语言支持模式
        UniGameResources.gameResourcesWorkModeTwo = UniGameResources.GameResourcesWorkModeTwo.Mode_OutGameOptions;
        //更换成当前的资源加载支持对象
        UniGameResources.uniGameResourcesSupport = new IUniGameResourcesSupport();
        //启动协同程序装载包清单文件
        UniGameResources.AllocResourcesDownLoadWorker();
        yield return StartCoroutine(UniGameResources.resourcesDownLoadWorker.LoadingGameAssetBundleInventoryPackageFile());
        //显示启动页面
        //yield return StartCoroutine(OpenGameBootFace());
        //检测当前程序版本是否符合资源版本的要求
        if (UniGameResources.produceInformation.SupportProgramVersion != UniGameResources.uniGameResourcesSupport.ProgramVersion)
        {
            bootingSystemStep = BootingSystemStep.Step_NeedInstallNewVersionProgram;
            yield break;
        }
        //装载要求强制装载的包,这些包不再卸载
        List<string> packageList = new List<string>(8);
        Dictionary<uint, UniGameResourcesPackage>.Enumerator list=UniGameResources.systemResourcesPackageTable.GetEnumerator();
        while(list.MoveNext())
        {
            if (list.Current.Value.assetBundleExistType == UniGameResourcesPackage.AssetBundleExistType.ExistType_ConstraintInstall)
            {
                packageList.Add(list.Current.Value.packageName);
            }
        }
        list.Dispose();
        if (packageList.Count != 0)
        {
            bootingSystemStep = BootingSystemStep.Step_ConstraintInstallPackage;
            constraintInstallDownloader = UniGameResources.resourcesDownLoadWorker.AllocResourcesDownLoader_DontDestroyOnLoad();
            yield return StartCoroutine(constraintInstallDownloader.DownloadPackageWithList(packageList));
        }
        //进行强制下载的资源包
        List<UniGameResourcesPackage> ConstraintDownloadPackageList = UniGameResources.ComparisonSystemResourcesPackageVersion();
        if (ConstraintDownloadPackageList.Count != 0)
        {
            bootingSystemStep = BootingSystemStep.Step_ConstraintDownloadPackage;
            constraintDownloadDownloader = UniGameResources.resourcesDownLoadWorker.AllocResourcesDownLoader();
            yield return StartCoroutine(constraintDownloadDownloader.DownloadPackageWithList(ConstraintDownloadPackageList));
            //下载完是需要卸载的
            UniGameResources.resourcesDownLoadWorker.ReleaseResourcesDownLoader(constraintDownloadDownloader);
            constraintDownloadDownloader = null;
        }
        //保存一次当前资源包版本
        UniGameResources.SaveSystemResourcesPackageVersion();
        //全部处理完毕
        bootingSystemStep = BootingSystemStep.Step_BootingOver;

        //示范代码下载并加载一个资源包内的场景
        //yield return StartCoroutine(UniGameResources.resourcesDownLoadWorker.LoadingGameSceneLevelAdditiveAsync("scene_game", "game"));

        //资源加载工作全部完成，开始初始化程序
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
        //开始自检过程
        GameRoot.gameProcessControl.ActivateProcess(typeof(SelfCheckingModalProcess));
    }
    
}
