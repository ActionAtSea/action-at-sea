////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - TileCaustics.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileCaustics : MonoBehaviour 
{
    public GameObject causticFrames = null;
    public GameObject baseFrame = null;

    private GameObject m_gameBoard = null;
    private float m_animationSpeed = 0.1f;
    private int m_tileAmountX = 0;
    private int m_tileAmountY = 0;
    private float m_timePassed = 0.0f;
    private List<GameObject> m_tiles = new List<GameObject>();
    private List<Sprite> m_frames = new List<Sprite>();
    private int m_currentTexture = 0;

    /// <summary>
    /// Initialises the caustics
    /// </summary>
    void Start ()
    {
        m_gameBoard = GameObject.FindGameObjectWithTag("GameBoard");
        if(m_gameBoard == null)
        {
            Debug.LogError("Could not find game board");
        }

        var causticFrame = causticFrames.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < causticFrame.Length; ++i)
        {
            causticFrame[i].enabled = false;
            m_frames.Add(causticFrame[i].sprite);
        }

        var renderer = baseFrame.GetComponent<SpriteRenderer>();
        var textureSize = renderer.sprite.texture.width;
        var worldScale = (float)textureSize / renderer.sprite.pixelsPerUnit;
        var scale = baseFrame.transform.localScale.x;
        worldScale *= scale;

        const int extraTiles = 2;
        var boardBounds = m_gameBoard.GetComponent<SpriteRenderer>().bounds;
        var boardWidth = Mathf.Abs(boardBounds.max.x - boardBounds.min.x);
        var boardLength = Mathf.Abs(boardBounds.max.y - boardBounds.min.y);
        m_tileAmountX = (int)Mathf.Ceil(boardWidth / worldScale) + extraTiles;
        m_tileAmountY = (int)Mathf.Ceil(boardLength / worldScale) + extraTiles;

        var totalTiles = m_tileAmountX * m_tileAmountY;
        for (int i = 0; i < totalTiles; ++i)
        {
            m_tiles.Add((GameObject)(Instantiate(baseFrame)));
            m_tiles[i].name = "Caustics" + i.ToString();
            m_tiles[i].transform.parent = this.transform;
            m_tiles[i].GetComponent<SpriteRenderer>().enabled = true;
            m_tiles[i].GetComponent<SpriteRenderer>().sprite = 
                baseFrame.GetComponent<SpriteRenderer>().sprite;
        }

        // Lay out the duplicates to form a grid, initial position centers it on 0,0
        Vector2 initialPosition = new Vector2 (
            worldScale * ((m_tileAmountX - 1) * 0.5f),
            worldScale * ((m_tileAmountY - 1) * 0.5f));

        for(int x = 0; x < m_tileAmountX; ++x)
        {
            for(int y = 0; y < m_tileAmountY; ++y)
            {
                var index = x * m_tileAmountX + y;
                m_tiles[index].transform.position = new Vector3(
                    initialPosition.x - (x * worldScale),
                    initialPosition.y - (y * worldScale),
                    0.0f);

                m_tiles[index].transform.localScale = new Vector3(scale, scale, 0.0f);
            }
        }
    }

    /// <summary>
    /// Updates the caustic animation
    /// </summary>
    void Update()
    {
        m_timePassed += Time.deltaTime;
        if(m_timePassed >= m_animationSpeed)
        {
            m_timePassed = 0.0f;

            ++m_currentTexture;
            if(m_currentTexture >= m_frames.Count)
            {
                m_currentTexture = 0;
            }

            for (int i = 0; i < m_tiles.Count; ++i)
            {
                m_tiles[i].GetComponent<SpriteRenderer>().sprite = m_frames[m_currentTexture];
            }
        }
    }
}
