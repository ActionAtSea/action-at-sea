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

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_score = startingScore;
        m_score = Mathf.Max(m_score, 0.0f);
        m_roundedScore = Mathf.Round(m_score);
    }

    /// <summary>
    /// Removes from the score
    /// </summary>
    public void MinusScore(float scoreToMinus)
    {
        m_score -= scoreToMinus;
        m_score = Mathf.Max(m_score, 0.0f);
        m_roundedScore = Mathf.Round(m_score);
    }

    /// <summary>
    /// Adds to the score
    /// </summary>
    public void AddScore(float scoreValue)
    {
        m_score += scoreValue;
        m_roundedScore = Mathf.Round(m_score);
    }

    /// <summary>
    /// Resets the score
    /// </summary>
    public void ResetScore()
    {
        m_score = 0.0f;
        m_roundedScore = 0.0f;
    }

    /// <summary>
    /// Gets the score
    /// </summary>
    public float Score
    {
        get { return m_score; }
    }

    /// <summary>
    /// Gets the rounded score
    /// </summary>
    public float RoundedScore
    {
        get { return m_roundedScore; }
    }
}