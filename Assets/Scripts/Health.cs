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
    private float m_healthMax = 100.0f;
    private float m_healthMin = 0.0f;
    private float m_healthLevel = 100.0f;
    private bool m_isAlive = true;

    /// <summary>
    /// Initialises the health
    /// Health bar can be either a floating or GUI bar
    /// </summary>
    void Start()
    {
        m_healthLevel = m_healthMax;

        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthBar = GameObject.FindWithTag("PlayerHealth");
            m_healthBar.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
        else
        {
            var floatingHealthBar = transform.parent.transform.FindChild("FloatingHealthBar");
            var canvas = floatingHealthBar.FindChild("Canvas").transform;
            m_healthBar = canvas.FindChild("HealthBar").gameObject;
            m_floatingHealthBarText = canvas.FindChild("PlayerName").gameObject;
        }

        if(m_healthBar == null)
        {
            Debug.LogError("Could not find health bar");
        }

        m_maxBarWidth = m_healthBar.GetComponent<RectTransform>().rect.width;
        m_barHeight = m_healthBar.GetComponent<RectTransform>().rect.height;
    }

    /// <summary>
    /// On Destroy set the GUI health back to normal
    /// </summary>
    void OnDestroy()
    {
        if(m_healthBar != null)
        {
            m_healthBar.GetComponent<RectTransform>().sizeDelta =
                new Vector2(m_maxBarWidth, m_barHeight);
        }
    }

    /// <summary>
    /// Updates the health
    /// </summary>
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
                m_healthBar.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
            else
            {
                m_healthBar.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(barWidth, m_barHeight);
            }
        }
    }

    /// <summary>
    /// Inflicts damage to the health
    /// </summary>
    public void InflictDamage(float damage)
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthLevel -= damage;
        }
    }

    /// <summary>
    /// Repairs damage to the health
    /// </summary>
    public void RepairDamage(float repairAmount)
    {
        if(NetworkedPlayer.IsControllable(gameObject))
        {
            m_healthLevel += repairAmount;
        }
    }

    /// <summary>
    /// Sets the health level
    /// </summary>
    public void SetHealthLevel(float level)
    {
        m_healthLevel = level;
    }

    /// <summary>
    /// Gets the health level
    /// </summary>
    public float HealthLevel
    {
        get { return m_healthLevel; }
    }

    /// <summary>
    /// Gets the maximum health
    /// </summary>
    public float HealthMax
    {
        get { return m_healthMax; }
    }

    /// <summary>
    /// Gets the minimum health
    /// </summary>
    public float HealthMin
    {
        get { return m_healthMin; }
    }

    /// <summary>
    /// Gets whether the health is alive
    /// </summary>
    public bool IsAlive
    {
        get { return m_isAlive; }
    }
}
