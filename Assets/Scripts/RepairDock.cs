////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - RepairDock.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class RepairDock : MonoBehaviour
{
    public float repairAmountPerSecond = 15.0f;
    public float scoreCostRate = 5.0f;

    /// <summary>
    /// On collision with the repair dock
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        if (PlayerManager.IsControllablePlayer(other.gameObject))
        {
            var health = other.gameObject.GetComponent<PlayerHealth>();
            if (health.HealthLevel < health.HealthMax)
            {
                var score = other.gameObject.GetComponent<PlayerScore>();
                if (score.Score > 0.0f)
                {
                    if (Input.GetKey("e"))
                    {
                        health.RepairDamage(repairAmountPerSecond * Time.deltaTime);
                        score.MinusScore(scoreCostRate * Time.deltaTime);
                        Debug.Log("health yes");
                    }
                }
            }
        }
    }
}