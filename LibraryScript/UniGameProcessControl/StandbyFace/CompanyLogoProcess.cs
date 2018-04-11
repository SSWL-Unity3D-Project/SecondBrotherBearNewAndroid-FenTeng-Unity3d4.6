using System;
using System.Collections.Generic;
using UnityEngine;
class CompanyLogoProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_CompanyLogo; } }
    public CompanyLogoProcess()
        : base()
    {

    }
    private GameObject CompanyLogobackground = null;
    private GuiPlaneAnimationPlayer CompanyLogoPlayer = null;
    private Texture[] CompanyLogoTexture = null;
    private int PlayIndex;
    //初始化函数
    public override void Initialization()
    {
        base.Initialization();
        //显示公司LOGO
        //版本说明界面等
        //最后进入游戏
        //载入贴图
        CompanyLogoTexture = new Texture[SystemCommand.CompanyLogoOrder.Length];
        for (int i = 0; i < CompanyLogoTexture.Length; i++)
        {
            CompanyLogoTexture[i] = GameRoot.gameResource.LoadLanguageResource_Texture(SystemCommand.CompanyLogoOrder[i], typeof(Texture));
        }
        CompanyLogobackground = GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("CompanyLogobackground.prefab", GameRoot.gameResource);
        CompanyLogoPlayer = GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("CompanyLogo.prefab", GameRoot.gameResource).GetComponent<GuiPlaneAnimationPlayer>();
        PlayerOneCompanyLogo(0);
    }
    //释放函数
    public override void Dispose()
    {
        if (CompanyLogobackground != null)
        {
            UnityEngine.Object.DestroyObject(CompanyLogobackground);
            CompanyLogobackground = null;
        }
        if (CompanyLogoPlayer != null)
        {
            UnityEngine.Object.DestroyObject(CompanyLogoPlayer.gameObject);
            CompanyLogoPlayer = null;
        }
        if (CompanyLogoTexture != null)
        {
            for (int i = 0; i < CompanyLogoTexture.Length; i++)
            {
                UniGameResources.ReleaseOneAssets(CompanyLogoTexture[i]);
            }
            CompanyLogoTexture = null;
        }

    }
    private void PlayerOneCompanyLogo(int index)
    {
        PlayIndex = index;
        MeshRenderer reader = CompanyLogoPlayer.GetComponent<MeshRenderer>();
        reader.material.mainTexture = CompanyLogoTexture[PlayIndex];

        CompanyLogoPlayer.DelegateOnPlayEndEvent = null;
        CompanyLogoPlayer.DelegateOnPlayEndEvent = OnOneCompanyLogoPlayEnd;
        CompanyLogoPlayer.Stop();
        CompanyLogoPlayer.Play();
    }
    private void OnOneCompanyLogoPlayEnd()
    {
        PlayIndex += 1;
        if (PlayIndex >= CompanyLogoTexture.Length)
        {
            //进入游戏待机过程需要一个独立过程，否则待机状态会被冲掉
            Dispose();
            ConsoleCenter.CurrentSceneWorkMode = SceneWorkMode.SCENEWORKMODE_Release;
            processControl.ActivateAloneProcess(typeof(StandbyProcess));
            Debug.Log("初始化StandByProcess模式");
            return;
        }
        PlayerOneCompanyLogo(PlayIndex);
    }

}
