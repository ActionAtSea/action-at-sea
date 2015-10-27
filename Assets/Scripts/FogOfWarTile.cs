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
    private MeshRenderer m_renderer = null;
    private Material m_material = null;
    private float m_alpha = 1.0f;
    private bool m_fade = false;   
    private bool m_static = false;  
    private float m_radius = 10.0f;                      

    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_material = GetComponent<MeshRenderer>().material;

        m_tintColor = new Color(
            m_tintColor.r,
            m_tintColor.g,
            m_tintColor.b,
            m_tintColor.a * UnityEngine.Random.Range(0.5f, 1.0f));

        m_material.SetColor("_TintColor", m_tintColor);
    }

    public float GetRadius()
    {
        return m_radius;
    }

    public bool IsStatic
    {
        set { m_static = value; }
    }

    public void CheckFadeOut()
    {
        if(!m_fade && PlayerManager.IsCloseToPlayer(transform.position.x, transform.position.z, m_radius))
        {
            m_fade = true;
        }
    }

    void Update()
    {
        if(m_renderer.isVisible)
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        if(!m_static && m_fade)
        {
            if(m_alpha <= 0.0f)
            {
                gameObject.SetActive(false);
            }
            else
            {
                m_alpha -= Time.deltaTime * 0.7f;
                m_material.SetColor("_TintColor", new Color(
                    m_tintColor.r, 
                    m_tintColor.g, 
                    m_tintColor.b, 
                    m_tintColor.a * m_alpha));
            }
        }
    }
}
