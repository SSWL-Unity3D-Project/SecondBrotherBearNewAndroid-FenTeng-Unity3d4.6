using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class UICoinsAnimControl : MonoBehaviourIgnoreGui
{

    public UICoinsAnimControl() { }

    public GuiPlaneAnimationPlayer m_GuiPlaneAnimationPlayer = null;
    public GuiPlaneAnimationText m_GuiPlaneAnimationText = null;

    //播放动画
    public void AnimPlay(string txt)
    {
        m_GuiPlaneAnimationPlayer.Stop();
        m_GuiPlaneAnimationText.Text = txt;
        m_GuiPlaneAnimationPlayer.Play();
    }


    //吃金币动画缓冲
    public static ObjectBufferT<UICoinsAnimControl> m_EatCoinsBuffer = new ObjectBufferT<UICoinsAnimControl>();

    //从金币缓冲表中获取金币
    public static UICoinsAnimControl GetCoinFromBuffer()
    {
        //从弹珠缓冲表中取弹珠， 如果没有取到弹珠则创建弹珠， 并加入到m_StaticPinballList里面
        UICoinsAnimControl newCoins = null;
        if (m_EatCoinsBuffer.AllocObject("AnimControl", out newCoins)) //是否在缓存列表中
        {

        }
        else
        {
            newCoins = UniGameResources.NormalizePrefabs(GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("Prompt_Coins.prefab", GameRoot.gameResource),
                                     null, typeof(UICoinsAnimControl)) as UICoinsAnimControl;
            //newCoins.m_GuiPlaneAnimationPlayer.DelegateOnPlayEndEvent = newCoins.fn;
        }
        newCoins.gameObject.SetActive(true);
        newCoins.m_GuiPlaneAnimationPlayer.DelegateOnPlayEndEvent = newCoins.TransToBuffer;
        return newCoins;
    }

    //把金币归还给缓冲列表
    public static void TrusteeshipCoinToBuffer(UICoinsAnimControl newCoins)
    {
        newCoins.m_GuiPlaneAnimationPlayer.Stop();
        newCoins.m_GuiPlaneAnimationText.Text = "";
        m_EatCoinsBuffer.TrusteeshipObject("AnimControl", newCoins);
        newCoins.gameObject.SetActive(false);
    }

    //添加入缓存列表
    public static void InitCoinsAnim()
    {
        for (int i = 0; i < 6; i++)
        {
            UICoinsAnimControl newCoins = GameRoot.uiOrthographicCamera.LoadResource_UIPrefabs("Prompt_Coins.prefab",GameRoot.gameResource).GetComponent<UICoinsAnimControl>();
            TrusteeshipCoinToBuffer(newCoins);
        }
    }

    public void TransToBuffer()
    {
        //this.m_GuiPlaneAnimationPlayer.Stop();
        TrusteeshipCoinToBuffer(this);
    }
}
