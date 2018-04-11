using System;
using System.Collections.Generic;
class SceneControlGame : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Game; } }
    protected override void LinkSceneInfo()
    {

    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        base.Initialization();
        //UniMessageBox.ShowWaring("怎么搞的啊!");
        //最后都初始化完成后取消启动画面
        if (SceneControl.gameBootFace != null)
        {
            SceneControl.gameBootFace.CloseGameBootFace();
            SceneControl.gameBootFace = null;
        }
    }
    protected override void OnDestroyScene()
    {
        base.OnDestroyScene();
    }
}
