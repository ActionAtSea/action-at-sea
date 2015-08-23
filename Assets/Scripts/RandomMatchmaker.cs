////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - RandomMatchmaker.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class RandomMatchmaker : Photon.PunBehaviour
{
    private bool m_joined = false;

    /**
    * Initialises the networking
    */
    void Start()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    /**
    * Gets the status of the connection
    */
    public string GetNetworkStatus()
    {
        return PhotonNetwork.connectionStateDetailed.ToString();
    }

    /**
    * Gets whether the network is connected
    */
    public bool IsConnected()
    {
        return m_joined;
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
        GameObject playerPrefab = PhotonNetwork.Instantiate("PlayerPVP", Vector3.zero, Quaternion.identity, 0);

        GameObject player = playerPrefab.transform.FindChild("Player").gameObject;
        player.tag = "Player";
        player.name = player.GetComponent<NetworkedPlayer>().PlayerID;
        player.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
        player.transform.position = FindObjectOfType<PlayerPlacer>().GetNewPosition();

        GameObject healthbar = playerPrefab.transform.FindChild("FloatingHealthBar").gameObject;
        healthbar.SetActive(false);

        m_joined = true;
    }

    /**
    * Gets the Network from the scene
    */
    public static RandomMatchmaker Get()
    {
        var network = FindObjectOfType<RandomMatchmaker>();
        if(network == null)
        {
            Debug.LogError("Could not find Random Matchmaker");
        }
        return network;
    }
}