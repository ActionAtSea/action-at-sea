////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CreateBackground.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class CreateBackground : MonoBehaviour
{
    void Start()
    {
        var gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(gameBoard == null)
        {
            Debug.LogError("Could not find game board");
        }

        if(gameBoard.transform.position.x != 0.0f ||
           gameBoard.transform.position.z != 0.0f)
        {
            Debug.LogError("Game board must be centered");
        }

        gameObject.transform.localRotation = gameBoard.transform.localRotation;

        gameObject.transform.localScale = new Vector3(
            gameBoard.transform.localScale.x * 3.0f,
            gameBoard.transform.localScale.y * 3.0f,
            gameBoard.transform.localScale.z * 3.0f);

        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        var colour = gameBoard.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(
            colour.r, colour.g, colour.b, colour.a);
    }
}