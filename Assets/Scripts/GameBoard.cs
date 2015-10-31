////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameBoard.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour 
{
    static GameObject m_gameBoard = null;

	void Start () 
    {
        GetComponent<SpriteRenderer>().enabled = false;
	}

    static public Bounds GetBounds()
    {
        return Get().GetComponent<SpriteRenderer>().bounds;
    }

    static public GameObject Get()
    {
        if(m_gameBoard == null)
        {
            m_gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
            if(m_gameBoard == null)
            {
                Debug.LogError("Could not find game board");
            }
        }
        return m_gameBoard;
    }
}
