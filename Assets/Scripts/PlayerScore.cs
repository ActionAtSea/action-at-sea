////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerScore.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
 * Handles the player score within a single level.
 */ 
public class PlayerScore : MonoBehaviour
{
    public float startingScore = 100.0f;
    private float m_score = 0.0f;
    private float m_roundedScore;

    /**
    * Initialises the script
    */
    void Start()
    {
        m_score = startingScore;
        m_score = Mathf.Max(m_score, 0.0f);
        m_roundedScore = Mathf.Round(m_score);
    }

    /**
    * Removes from the score
    */
    public void MinusScore(float scoreToMinus)
    {
        m_score -= scoreToMinus;
        m_score = Mathf.Max(m_score, 0.0f);
        m_roundedScore = Mathf.Round(m_score);
    }

    /**
    * Adds to the score
    */
    public void AddScore(float scoreValue)
    {
        m_score += scoreValue;
        m_roundedScore = Mathf.Round(m_score);
    }

    /**
    * Resets the score
    */
    public void ResetScore()
    {
        m_score = 0.0f;
        m_roundedScore = 0.0f;
    }

    /**
    * Gets the score
    */
    public float Score
    {
        get { return m_score; }
    }

    /**
    * Gets the rounded score
    */
    public float RoundedScore
    {
        get { return m_roundedScore; }
    }
}