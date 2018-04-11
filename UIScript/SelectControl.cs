using UnityEngine;
using System.Collections;

public class SelectControl : MonoBehaviourIgnoreGui
{
    //选择和未选择的
    public SelectChildObjectControl m_SelectControl = null;
    public SelectChildObjectControl m_StandbyControl = null;

    internal enum UIState
    {
        Ini, //初始化状态 
        Show //已显示状态
    }
    internal UIState m_UIState = UIState.Ini;

    public virtual void Initialization()
    {
        m_UIState = UIState.Ini;
        m_SelectControl.Initialization();
        m_StandbyControl.Initialization();
    }

    //界面显示状态
    public virtual void ControlUI(bool select)
    {
        if (select)
        {
            m_SelectControl.gameObject.SetActive(true);
            m_StandbyControl.gameObject.SetActive(false);
            m_UIState = UIState.Show;
        }
        else
        {
            m_StandbyControl.gameObject.SetActive(true);
            m_SelectControl.gameObject.SetActive(false);
            m_UIState = UIState.Ini;
        }
    }
}
