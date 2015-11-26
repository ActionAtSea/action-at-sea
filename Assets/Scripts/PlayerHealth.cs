using UnityEngine;
using System.Collections;

public class PlayerHealth : Health 
{

	// Use this for initialization
	void Start () 
    {
        m_healthLevel = m_healthMax;
	}

    protected override void Initialise()
    {
        if(Utilities.IsPlayerControllable(gameObject))
        {
            m_healthBar = GameObject.FindWithTag("PlayerHealth");
            m_healthBar.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
        else
        {
            var floatingHealthBar = transform.parent.transform.FindChild("FloatingHealthBar");
            var canvas = floatingHealthBar.FindChild("Canvas").transform;
            m_healthBar = canvas.FindChild("HealthBar").gameObject;
            var playerName = canvas.FindChild("PlayerName").gameObject;
            m_floatingHealthBarText = playerName.GetComponent<UnityEngine.UI.Text>();
        }
        
        if(m_healthBar == null)
        {
            Debug.LogError("Could not find health bar");
        }
        
        m_maxBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
        m_barHeight = m_healthBar.GetComponent<RectTransform>().rect.height;
        m_initialised = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if(!Utilities.IsLevelLoaded())
        {
            return;
        }
        
        if(!m_initialised)
        {
            Initialise();
        }
        
        if(m_floatingHealthBarText != null)
        {
            m_floatingHealthBarText.text = 
                Utilities.GetPlayerName(gameObject);
        }
        
        if (m_healthLevel > m_healthMax)
        {
            m_healthLevel = m_healthMax;
        }
        
        if (m_healthLevel <= m_healthMin)
        {
            m_healthLevel = 0.0f;
            m_isAlive = false;
        }
        
        // If the object has a health bar scale it to show the health
        if(m_healthBar != null)
        {
            // Convert the value range from 0->100 to 0->maxBarScale
            float barWidth = ((m_healthLevel-m_healthMin)*((m_maxBarWidth-
                                                            m_minBarWidth)/(m_healthMax-m_healthMin)))+m_minBarWidth;
            
            m_healthBar.GetComponent<UnityEngine.UI.Image>().enabled = barWidth > m_minBarWidth;
            m_healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, m_barHeight);
        }
	}

    void OnDestory()
    {
        if(m_healthBar != null)
        {
            m_healthBar.GetComponent<RectTransform>().sizeDelta =
                new Vector2(m_maxBarWidth, m_barHeight);
        }
    }
}
