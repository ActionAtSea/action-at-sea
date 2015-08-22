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

    private float m_animationSpeed = 0.1f;
    private int m_tileAmountX = 11;
    private int m_tileAmountY = 11;
    private float m_timePassed = 0.0f;
    private List<GameObject> m_tiles = new List<GameObject>();
    private List<Sprite> m_frames = new List<Sprite>();
    private int m_currentTexture = 0;

    /**
    * Initialises the caustics
    */
    void Start ()
    {
        var causticFrame = causticFrames.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < causticFrame.Length; ++i)
        {
            causticFrame[i].enabled = false;
            m_frames.Add(causticFrame[i].sprite);
        }

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

        var renderer = baseFrame.GetComponent<SpriteRenderer>();
        var textureSize = renderer.sprite.texture.width;
        var worldScale = (float)textureSize / renderer.sprite.pixelsPerUnit;
        var scale = baseFrame.transform.localScale.x;
        worldScale *= scale;

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

    /**
    * Updates the caustic animation
    */
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
