////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUIMaxPlayers.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

public class GUIMaxPlayers : MonoBehaviour 
{
    public UnityEngine.UI.Text m_text;
    private UnityEngine.UI.Slider m_slider;

    void Start()
    {
        m_slider = GetComponent<UnityEngine.UI.Slider>();
        if (m_slider == null)
        {
            Debug.LogError("Could not find slider");
        }
    }

    void Update()
    {
        Utilities.SetMaximumPlayers((int)m_slider.value);
        m_text.text = ((int)m_slider.value).ToString();
    }

    public int GetMaxPlayers()
    {
        return (int)m_slider.value;
    }

    public void SetEnabled(bool value)
    {
        m_slider.interactable = value;
    }

    public void SetMaxPlayers(int value)
    {
        m_slider.maxValue = value;
        m_slider.value = (float)value;
    }
}
