using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Handles the purchasing of player owned AI ships and ship upgrades.
/// </summary>
public class ShopManager : MonoBehaviour
{

    /*
     * TODO: Add tool tips showing the hotkey and once 
     * each button does whilst the mouse is hovering
     * over them. Also disable cannon firing whilst mouse
     * is hovering over buttons.
     */

    public float fleetShipCost;
    public float patrolShipCost;

    public Button fleetButton;
    public Button patrolButton;
    public Button cannonButton;

    private GameObject fleetAI = null;
    private SoundManager soundManager = null;
    private GameObject player = null;
    private NetworkedPlayer networkedPlayer = null;
    private PlayerScore playerScore = null;

    private float buttonPressCooldown = 0.4f;
    private float fleetButtonTimer = 0.0f;
    private float patrolButtonTimer = 0.0f;
    private float cannonButtonTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        fleetButton.interactable = false;
        patrolButton.interactable = false;
        cannonButton.interactable = false;
        soundManager = SoundManager.Get();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null & Utilities.IsLevelLoaded() && !Utilities.IsGameOver())
        {
            player = PlayerManager.GetControllablePlayer();
            if(player != null)
            {
                networkedPlayer = player.GetComponent<NetworkedPlayer>();
                playerScore = player.GetComponent<PlayerScore>();
            }

            
        }

        if (networkedPlayer != null)
        {
            if (networkedPlayer.IslandWithinRange != null)
            {
                fleetButton.interactable = true;
                patrolButton.interactable = true;
                //TODO: Enable cannonButton once cannon upgrades have been implemented.
                //cannonButton.interactable = true;
            }
            else
            {
                fleetButton.interactable = false;
                patrolButton.interactable = false;
                cannonButton.interactable = false;
            }
        }

        
        if (fleetButton.IsInteractable() && Input.GetAxis("BuyFleetShip") == 1 && fleetButtonTimer >=buttonPressCooldown)
        {
            FleetButtonPress();
            fleetButtonTimer = 0.0f;
        }

        if (patrolButton.IsInteractable() && Input.GetAxis("BuyPatrolShip") == 1 && patrolButtonTimer >= buttonPressCooldown)
        {
            PatrolButtonPress();
            patrolButtonTimer = 0.0f;
        }

        //TODO: Add cannon upgrade button.
        if (fleetButtonTimer < buttonPressCooldown)
        {
            fleetButtonTimer += Time.deltaTime;
        }
        if (patrolButtonTimer < buttonPressCooldown)
        {
            patrolButtonTimer += Time.deltaTime;
        }
    }

    public void FleetButtonPress()
    {
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }
        Debug.Log("FleetButtonPress.");
    }

    public void PatrolButtonPress()
    {
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }
        Debug.Log("PatrolButtonPress.");
    }

    public void CannonButtonPress()
    {
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }
        Debug.Log("CannonButtonPress");
    }
}