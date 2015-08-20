using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
 * Handles the player score within a single level.
 */ 
public class PlayerScore : MonoBehaviour
{
    public float startingScore = 100.0f;
    private float score = 0.0f;
    private float roundedScore;

    // Use this for initialization
    void Start()
    {
        score = startingScore;
    }

    // Update is called once per frame
    void Update()
    {
        if (score < 0.0f)
        {
            score = 0.0f;
        }

        roundedScore = Mathf.Round(score);
    }

    public void MinusScore(float scoreToMinus)
    {
        score -= scoreToMinus;
    }

    public void AddScore(float scoreValue)
    {
        score += scoreValue;
    }

    public void ResetScore()
    {
        score = 0.0f;
    }

    public float Score
    {
        get { return score; }
    }

	public float RoundedScore
	{
		get { return roundedScore; }
	}
}