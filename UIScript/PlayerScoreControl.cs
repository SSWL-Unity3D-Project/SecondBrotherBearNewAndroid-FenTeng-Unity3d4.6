using UnityEngine;
using System.Collections;

public class PlayerScoreControl : MonoBehaviourIgnoreGui
{

    //分数 按顺序 熊大 熊2
    public GameObject[] m_PlayerScoreObject = new GameObject[2];
    private GuiPlaneAnimationTextAdvanced[] m_PlayerScore = new GuiPlaneAnimationTextAdvanced[2];

    public void Initialization()
    {
        InitUI();
        GetScore();
    }

    private void InitUI()
    {
        for (int i = 0; i < m_PlayerScoreObject.Length; i++)
        {
            m_PlayerScore[i] = m_PlayerScoreObject[i].GetComponent<GuiPlaneAnimationTextAdvanced>();
        }
    }

    //获取分数
    public void GetScore()
    {
        m_PlayerScore[0].Text = ((RaceSceneControl)GameRoot.CurrentSceneControl).XiongDaScore.ToString();
        m_PlayerScore[1].Text = ((RaceSceneControl)GameRoot.CurrentSceneControl).Xiong2Score.ToString();
    }

}
