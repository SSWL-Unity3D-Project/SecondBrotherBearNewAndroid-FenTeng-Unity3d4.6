using UnityEngine;
using System.Collections;

public class SelectChildObjectControl : MonoBehaviourIgnoreGui
{
    //熊Bg动画控制
    public GameObject m_BackgroundPlaneGameObject = null;
    internal GuiPlaneAnimationPlayer m_BackgroundPlaneAnimationPlayer = null;

    public void Initialization()
    {
        m_BackgroundPlaneAnimationPlayer = m_BackgroundPlaneGameObject.GetComponent<GuiPlaneAnimationPlayer>();
        //Debug.Log("动画播放脚本 初始化：：：" + m_BackgroundPlaneAnimationPlayer);
    }

}
