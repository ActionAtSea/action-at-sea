////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - FogOfWarTile.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarTile : MonoBehaviour 
{   
    public Color m_tintColor;
    private Material m_material = null;
    private float m_alpha = 1.0f;
    private bool m_fade = false;   
    private bool m_static = false;  
    private float m_radius = 10.0f;                      
    private float m_rotation = 10.0f;

    void Start()
    {
        m_material = GetComponent<MeshRenderer>().material;
    }

    public bool IsStatic
    {
        set { m_static = value; }
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(
            Camera.main.transform.eulerAngles.x + m_rotation,
            Camera.main.transform.eulerAngles.y,
            Camera.main.transform.eulerAngles.z);

        if(!m_static)
        {
            if(m_fade)
            {
                if(m_alpha <= 0.0f)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    m_alpha -= Time.deltaTime * 0.7f;
                    m_material.SetColor("_TintColor",
                        new Color(m_tintColor.r, m_tintColor.g, m_tintColor.b, m_tintColor.a * m_alpha));
                }
            }
            else
            {
                var player = PlayerManager.GetControllablePlayer();
                if(player != null && Utilities.IsPlayerInitialised(player))
                {
                    m_fade = PlayerManager.IsCloseToPlayer(
                        transform.position.x, transform.position.z, m_radius);
                }
            }
        }
    }
}
