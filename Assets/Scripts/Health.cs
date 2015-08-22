////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Health.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    private GameObject m_healthBar = null;
    private GameObject m_floatingHealthBarText = null;
    private float m_minBarWidth = 0.0f;
    private float m_maxBarWidth = 0.0f;
    private float m_barHeight = 0.0f;
    private bool m_hasScreenSpaceBar = false;
    private float m_healthMax = 100.0f;
    private float m_healthMin = 0.0f;
    private float m_healthLevel;
    private bool m_isAlive = true;

    /**
    * Initialises the health
    */
    void Start()
    {
        m_healthLevel = m_healthMax;

        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthBar = GameObject.FindWithTag("PlayerHealth");
        }
        else
        {
            var floatingHealthBar = transform.parent.transform.FindChild("FloatingHealthBar");
            m_healthBar = floatingHealthBar.FindChild("HealthBar").gameObject;
            m_floatingHealthBarText = floatingHealthBar.FindChild("Canvas").FindChild("Text").gameObject;
        }

        if(m_healthBar.GetComponent<RectTransform>() != null)
        {
            m_hasScreenSpaceBar = true;
            m_maxBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
            m_barHeight = m_healthBar.GetComponent<RectTransform>().rect.height;
        }
        else
        {
            m_maxBarWidth = m_healthBar.transform.localScale.x;
            m_barHeight = m_healthBar.transform.localScale.y;
        }
    }

    /**
    * Updates the health
    */
    void Update()
    {
        if(m_floatingHealthBarText != null)
        {
            m_floatingHealthBarText.GetComponent<UnityEngine.UI.Text>().text = 
                NetworkedPlayer.GetPlayerName(gameObject);
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

            if(barWidth <= m_minBarWidth)
            {
                m_healthBar.SetActive(false);
            }
            else
            {
                if(m_hasScreenSpaceBar)
                {
                    m_healthBar.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(barWidth, m_barHeight);
                }
                else
                {
                    m_healthBar.transform.localScale = new Vector3(
                        barWidth, m_barHeight, 0.0f);
                }
            }
        }
    }

    /**
    * Inflicts damage to the health
    */
    public void InflictDamage(float damage)
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthLevel -= damage;
        }
    }

    /**
    * Repairs damage to the health
    */
    public void RepairDamage(float repairAmount)
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthLevel += repairAmount;
        }
    }

    /**
    * Sets the health level
    */
    public void SetHealthLevel(float level)
    {
        m_healthLevel = level;
    }

    /**
    * Gets the health level
    */
    public float HealthLevel
    {
        get { return m_healthLevel; }
    }

    /**
    * Gets the maximum health
    */
    public float HealthMax
    {
        get { return m_healthMax; }
    }

    /**
    * Gets the minimum health
    */
    public float HealthMin
    {
        get { return m_healthMin; }
    }

    /**
    * Gets whether the health is alive
    */
    public bool IsAlive
    {
        get { return m_isAlive; }
    }
}
