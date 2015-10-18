////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FillScoreBoard.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FillScoreBoard : MonoBehaviour 
{
    public GameObject scoreText;
    private List<UnityEngine.UI.Text> m_scoreFront = new List<UnityEngine.UI.Text>();
    private List<UnityEngine.UI.Text> m_scoreBack = new List<UnityEngine.UI.Text>();

    /// <summary>
    /// Instantiates player text
    /// </summary>
    void Start()
    {
        AddText(scoreText);
        scoreText.SetActive(false);
        float xPosition = scoreText.GetComponent<RectTransform>().localPosition.x;
        float yPosition = scoreText.GetComponent<RectTransform>().localPosition.y;

        var level = Utilities.GetLoadedLevel();
        int amount = Utilities.GetMaxPlayersForLevel(level);

        for(int i = 1; i < amount; ++i)
        {
            var obj = Instantiate(scoreText);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.GetComponent<RectTransform>().localScale =
                scoreText.GetComponent<RectTransform>().localScale;

            obj.GetComponent<RectTransform>().localPosition = new Vector3(
                xPosition, 
                yPosition - (i * 15.0f),
                0.0f);

            AddText(obj);
        }
    }

    /// <summary
    /// Instantiates player text
    /// </summary>
    void AddText(GameObject obj)
    {
        var frontText = obj.transform.GetChild(0);
        m_scoreBack.Add(obj.GetComponent<UnityEngine.UI.Text>());
        m_scoreFront.Add(frontText.GetComponent<UnityEngine.UI.Text>());
    }

    /// <summary>
    /// Compiles a score board from all game players
    /// </summary>
    void Update () 
    {
        foreach(var obj in m_scoreBack)
        {
            obj.gameObject.SetActive(false);
        }

        List<GameObject> players = PlayerManager.GetAllPlayersByScore();
        int maxShown = Mathf.Min(players.Count, Utilities.GetMaxLevels());

        for(int i = 0; i < maxShown; ++i)
        {
            var obj = players[i];
            if(obj != null)
            {
                m_scoreBack[i].gameObject.SetActive(true);

                m_scoreFront[i].color = Utilities.GetPlayerColor(obj);
                m_scoreBack[i].text = Utilities.GetPlayerScore(obj).ToString() + ": " +
                    Utilities.GetPlayerName(obj);
                m_scoreFront[i].text = m_scoreBack[i].text;
            }

        }
    }
}
