using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserPrefTest : MonoBehaviour {
    [SerializeField]
    Text m_text;

    [SerializeField]
    Text m_scoreListText;

    List<int> m_scoreList = new List<int>();
    int m_count = 0;
    
    void Start()
    {
        // スコアリストの読み込み
        m_scoreList.Add(PlayerPrefs.GetInt("1st", 0));
        m_scoreList.Add(PlayerPrefs.GetInt("2nd", 0));
        m_scoreList.Add(PlayerPrefs.GetInt("3rd", 0));

        ReFreshScoreList();
    }

	void Update()
    {
        // Increment m_count
	    if (Input.GetKeyDown(KeyCode.Return))
        {
            m_count++;
        }

        // Reset Scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Save
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_scoreList.Add(m_count);
            ReFreshScoreList();
        }

        // Load
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Load Data");
        }

        m_text.text = m_count.ToString();
	}

    void ReFreshScoreList()
    {
        
        m_scoreList.Sort();
        m_scoreList.Reverse();
        if (m_scoreList.Count > 3)
        {
            m_scoreList.RemoveRange(3, m_scoreList.Count - 3);
        }

        

        // スコア表の更新
        string m_scoreListString = "";
        m_scoreListString += "1: " + m_scoreList[0] + "\n";
        m_scoreListString += "2: " + m_scoreList[1] + "\n";
        m_scoreListString += "3: " + m_scoreList[2];

        m_scoreListText.text = m_scoreListString;

        // スコアのセーブ
        PlayerPrefs.SetInt("1st", m_scoreList[0]);
        PlayerPrefs.SetInt("2nd", m_scoreList[1]);
        PlayerPrefs.SetInt("3rd", m_scoreList[2]);
    }
}
