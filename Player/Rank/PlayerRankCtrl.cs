using System.Collections.Generic;
using UnityEngine;

public class PlayerRankCtrl : MonoBehaviourIgnoreGui
{
    public List<IParkourPlayer_Xiong> m_ListPlayer;
    float mTimeLastRank = 0f;
	// Update is called once per frame
	void Update ()
    {
        CheckPlayerRank();
    }

    /// <summary>
    /// 排序比较器.
    /// </summary>
    int CompareRankDt(IParkourPlayer_Xiong x, IParkourPlayer_Xiong y)//排序器  
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            return 1;
        }

        if (y == null)
        {
            return -1;
        }

        int retval = 0;
        Vector3 forward_X = x.transform.right;
        Vector3 pos_X = x.transform.position;
        Vector3 pos_Y = y.transform.position;
        Vector3 vecXY = pos_Y - pos_X;
        float dotVal = Vector3.Dot(forward_X, vecXY);
        if (dotVal > 0f)
        {
            //x在y的后面.
            retval = 1;
        }
        else if (dotVal < 0f)
        {
            //x在y的前面.
            retval = -1;
        }
        //Debug.Log("rv == " + retval + ", x == " + x.name + ", y == " + y.name);
        return retval;
    }

    /// <summary>
    /// 检测玩家排名状态.
    /// </summary>
    void CheckPlayerRank()
    {
        if (Time.time - mTimeLastRank < 0.3f)
        {
            return;
        }
        mTimeLastRank = Time.time;

        if (m_ListPlayer.Count < 2)
        {
            return;
        }

        m_ListPlayer.Sort(CompareRankDt);
        for (int i = 0; i < m_ListPlayer.Count; i++)
        {
            if (m_ListPlayer[i].PlayerRankObj == null)
            {
                continue;
            }

            if (i == 0)
            {
                if (!m_ListPlayer[i].PlayerRankObj.activeInHierarchy)
                {
                    m_ListPlayer[i].PlayerRankObj.SetActive(true);
                }
            }
            else
            {
                if (m_ListPlayer[i].PlayerRankObj.activeInHierarchy)
                {
                    m_ListPlayer[i].PlayerRankObj.SetActive(false);
                }
            }
        }
    }
}