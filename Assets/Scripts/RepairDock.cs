using UnityEngine;
using System.Collections;

public class RepairDock : MonoBehaviour
{
    public float repairAmountPerSecond = 15.0f;
    public float scoreCostRate = 5.0f;
    //private Player player;
    private Health playerHealth;
    private GameObject player;
    private PlayerScore score;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
		if(player == null)
		{
			player = GameObject.FindWithTag("Player");
			if(player != null)
			{
				playerHealth = player.GetComponent<Health>();
				if (!playerHealth)
				{
					Debug.LogError("Repair Dock could not find Player Health.");
				}
				
				score = player.GetComponent<PlayerScore>();
				if (!score)
				{
					Debug.LogError("Repair Dock could not find PlayerScore.");
				}
			}
		}
    }

    void OnTriggerStay2D(Collider2D other)
    {
		if(player == null)
		{
			return;
		}

        if (other.gameObject.tag == "Player")
        {
            if (!playerHealth)
            {
                other.gameObject.GetComponent<Health>();
                //Debug.Log("yes it works");
            }
            else
            {
                if (playerHealth.HealthLevel < playerHealth.HealthMax)
                {
                    if (score.Score > 0.0f)
                    {
                        if (Input.GetKey("e"))
                        {
                            playerHealth.RepairDamage(repairAmountPerSecond * Time.deltaTime);
                            score.MinusScore(scoreCostRate * Time.deltaTime);
                            Debug.Log("health yes");
                        }
                    }
                }
            }
        }
    }
}