////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkMatchmaker.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkMatchmaker : Photon.PunBehaviour
{
    /// <summary>
    /// Initialises the networking
    /// </summary>
    void Start()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        if(!IsConnected())
        {
            PhotonNetwork.ConnectUsingSettings(Utilities.GameVersion());
        }
    }

    /// <summary>
    /// Gets the status of the connection
    /// </summary>
    public string GetNetworkStatus()
    {
        return PhotonNetwork.connectionStateDetailed.ToString();
    }

    /// <summary>
    /// Gets whether the network is connected to a room
    /// </summary>
    public bool IsConnectedToRoom()
    {
        return PhotonNetwork.inRoom;
    }

    /// <summary>
    /// Gets whether the network is connected to a room
    /// </summary>
    public bool IsConnected()
    {
        return PhotonNetwork.insideLobby ||
            IsConnectedToRoom();
    }

    /// <summary>
    /// On player joining the lobby
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
        if(Utilities.IsLevelLoaded() && !IsConnectedToRoom())
        {
            JoinGameRoom(Utilities.GetLoadedLevel());
        }
    }

    /// <summary>
    /// Player attempts to join a room
    /// </summary>
    public void JoinGameRoom(int level)
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// On player joining a new room
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        CreatePlayer();
    }

    /// <summary>
    /// On player failed to join room
    /// </summary>
    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Could not find room");
        PhotonNetwork.CreateRoom(null);
    }

    /// <summary>
    /// Creates a new player
    /// </summary>
    void CreatePlayer()
    {
        GameObject playerPrefab = PhotonNetwork.Instantiate("PlayerPVP", Vector3.zero, Quaternion.identity, 0);
        
        var networkedPlayer = playerPrefab.transform.GetComponentInChildren<NetworkedPlayer>();
        var player = networkedPlayer.gameObject;
        
        player.tag = "Player";
        player.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
        
        PlayerPlacer.Placement place = FindObjectOfType<PlayerPlacer>().GetNewPosition(player);
        player.transform.position = place.position;
        player.transform.localEulerAngles = place.rotation;
        
        GameObject healthbar = playerPrefab.transform.FindChild("FloatingHealthBar").gameObject;
        healthbar.SetActive(false);
    }

    /// <summary>
    /// Gets the Network from the scene
    /// </summary>
    public static NetworkMatchmaker Get()
    {
        var network = FindObjectOfType<NetworkMatchmaker>();
        if(network == null)
        {
            Debug.LogError("Could not find Random Matchmaker");
        }
        return network;
    }
}