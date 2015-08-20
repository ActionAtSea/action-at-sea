using UnityEngine;
using System.Collections;

public class EnemyAiming : MonoBehaviour
{


    private Vector3 playerPosition;
    //private Player player;
    private CannonController controller;


    // Use this for initialization
    void Start()
    {
        controller = GetComponentInChildren<CannonController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("player within range");
            playerPosition = other.transform.position;
            controller.AimWeapon(playerPosition);
            controller.FireWeapon();
        }
    }
}