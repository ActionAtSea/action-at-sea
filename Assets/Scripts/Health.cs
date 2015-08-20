using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour, IDamageable
{
	public GameObject healthBar = null;
	private GameObject floatingHealthBarText = null;
	public bool controllable = false;
	private bool initialised = false;
	private float minBarWidth = 0.0f;
	private float maxBarWidth = 0.0f;
	private float barHeight = 0.0f;
	private bool hasScreenSpaceBar = false;
    private float healthMax = 100.0f;
    private float healthMin = 0.0f;
    private float healthLevel;
    private bool isAlive = true;

	bool HasHealthBar()
	{
		return healthBar != null;
	}

	void InitialiseHealthBar()
	{
		if(!HasHealthBar())
		{
			if((!GameInformation.IsPVP() || (controllable && GameInformation.IsPVP())))
			{
				var player = GameObject.FindWithTag("Player");
				if(player != null)
				{
					healthBar = GameObject.FindWithTag("PlayerHealth");
				}
			}
			else if(!controllable && GameInformation.IsPVP())
			{
				var floatingHealthBar = transform.parent.transform.FindChild("FloatingHealthBar");
				healthBar = floatingHealthBar.FindChild("HealthBar").gameObject;
				floatingHealthBarText = floatingHealthBar.FindChild("Canvas").FindChild("Text").gameObject;
			}
		}

		if(HasHealthBar() && !initialised)
		{
			initialised = true;

			if(healthBar.GetComponent<RectTransform>() != null)
			{
				hasScreenSpaceBar = true;
				maxBarWidth = healthBar.GetComponent<RectTransform>().rect.width;
				barHeight = healthBar.GetComponent<RectTransform>().rect.height;
			}
			else
			{
				maxBarWidth = healthBar.transform.localScale.x;
				barHeight = healthBar.transform.localScale.y;
			}
		}
	}

    // Use this for initialization
    void Start()
    {
        healthLevel = healthMax;
    }
	
    // Update is called once per frame
    void Update()
    {
		if(isAlive)
		{
			InitialiseHealthBar();
		}

		if(floatingHealthBarText != null)
		{
			floatingHealthBarText.GetComponent<UnityEngine.UI.Text>().text = 
				gameObject.GetComponent<NetworkedPlayer>().PlayerName;
		}

        if (healthLevel > healthMax)
        {
            healthLevel = healthMax;
        }

        if (healthLevel <= healthMin)
        {
            healthLevel = 0.0f;
            isAlive = false;
        }

		// If the object has a health bar scale it to show the health
		if(HasHealthBar())
		{
			// Convert the value range from 0->100 to 0->maxBarScale
			float barWidth = ((healthLevel-healthMin)*((maxBarWidth-
				minBarWidth)/(healthMax-healthMin)))+minBarWidth;

			if(barWidth <= minBarWidth)
			{
				healthBar.SetActive(false);
			}
			else
			{
				if(hasScreenSpaceBar)
				{
					healthBar.GetComponent<RectTransform>().sizeDelta =
						new Vector2(barWidth, barHeight);
				}
				else
				{
					healthBar.transform.localScale = new Vector3(
						barWidth, barHeight, 0.0f);
				}
			}
		}
    }

    public void InflictDamage(float damage)
    {
		if(!GameInformation.IsPVP() || (controllable && GameInformation.IsPVP()))
		{
        	healthLevel -= damage;
		}
    }

    public void RepairDamage(float repairAmount)
    {
		if(!GameInformation.IsPVP() || (controllable && GameInformation.IsPVP()))
        {
        	healthLevel += repairAmount;
		}
    }

	public void SetHealthLevel(float level)
	{
		healthLevel = level;
	}

    public float HealthLevel
    {
        get { return healthLevel; }
    }

    public float HealthMax
    {
        get { return healthMax; }
    }

    public float HealthMin
    {
        get { return healthMin; }
    }

    public bool IsAlive
    {
        get { return isAlive; }
    }
}
