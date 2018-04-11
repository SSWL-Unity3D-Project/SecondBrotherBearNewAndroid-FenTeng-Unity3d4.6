using UnityEngine;
using System.Collections;

public class GameContinueControl : MonoBehaviourIgnoreGui
{

    //倒计时预制
    public GameObject m_Prompt_ContinueObject = null;
    internal GuiPlaneAnimationPlayer m_Prompt_Continue = null;
    //倒计时时间
    public float m_ContinueTime = 0.0f;

    //游戏结束预制
    public GameObject m_Prompt_GameOverObject = null;
    internal GuiPlaneAnimationPlayer m_Prompt_GameOver = null;
    //游戏结束的时间
    public float m_GameOverTime = 0.0f;

    //初始化函数
    public void Initialization()
    {
        m_Prompt_Continue = m_Prompt_ContinueObject.GetComponent<GuiPlaneAnimationPlayer>();
        m_Prompt_GameOver = m_Prompt_GameOverObject.GetComponent<GuiPlaneAnimationPlayer>();
        InitUI();
    }

    //初始化界面
    public void InitUI()
    {
        Invoke("ShowGameOver", m_ContinueTime);

        m_Prompt_Continue.gameObject.SetActive(false);
        m_Prompt_GameOver.gameObject.SetActive(false);

        m_Prompt_Continue.gameObject.SetActive(true);
        m_Prompt_Continue.Stop();
        m_Prompt_Continue.Play();

    }

    //显示游戏结束
    private void ShowGameOver()
    {
        m_Prompt_Continue.gameObject.SetActive(false);
        m_Prompt_GameOver.gameObject.SetActive(true);
        m_Prompt_GameOver.Stop();
        m_Prompt_GameOver.Play();
    }

    //终止显示游戏结束的倒计时--提前跳出继续界面时使用
    public void StopShowGameOver()
    {
        CancelInvoke("ShowGameOver");
    }
}
