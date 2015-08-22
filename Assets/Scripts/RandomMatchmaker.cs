////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - RandomMatchmaker.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class RandomMatchmaker : Photon.PunBehaviour
{
    /**
    * Initialises the networking
    */
    void Start()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    /**
    * Updates the status of the connection
    */
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    /**
    * On player joining the lobby
    */
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        PhotonNetwork.JoinRandomRoom();
    }

    /**
    * On player failed to join room
    */
    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Could not find room, creating new room");
        PhotonNetwork.CreateRoom(null);
    }

    /**
    * On player joining a new room
    */
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        GameObject playerPVP = PhotonNetwork.Instantiate("PlayerPVP", Vector3.zero, Quaternion.identity, 0);

        GameObject player = playerPVP.transform.FindChild("Player").gameObject;
        player.tag = "Player";
        player.GetComponent<NetworkedPlayer>().PlayerID = "Player";

        player.GetComponent<PlayerAiming>().controllable = true;
        player.GetComponent<PlayerMovement>().controllable = true;
        player.GetComponent<Health>().controllable = true;
        player.transform.FindChild("Cannons").gameObject.GetComponent<CannonController>().controllable = true;

        player.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
        player.transform.position = FindObjectOfType<PlayerPlacer>().GetNewPosition();

        GameObject healthbar = playerPVP.transform.FindChild("FloatingHealthBar").gameObject;
        healthbar.SetActive(false);
    }
}