using System;
using System.Collections.Generic;
using System.Text;

//场景控制部分
partial class GameRoot
{
    /********************场景注册*********************************/
    //当前关联的场景组件
    public static SceneControl CurrentSceneControl { set; get; }
    //注册当前场景
    public static void RegisterScene(SceneControl lpCurrentSceneControl)
    {
        if (!isRunedGameEnvironmentInitialization)
        {
            GameEnvironmentInitialization();
            isRunedGameEnvironmentInitialization = true;
        }
        CurrentSceneControl = lpCurrentSceneControl;
        CurrentSceneControl.Initialization();
    }
    //注销当前场景
    public static void UnRegisterScene(SceneControl lpCurrentSceneControl)
    {
        if (CurrentSceneControl == lpCurrentSceneControl)
            CurrentSceneControl = null;
    }
}
