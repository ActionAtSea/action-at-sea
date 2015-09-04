////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkMatchmaker.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMatchmaker : Photon.PunBehaviour
{
    DisconnectCause? m_disconnectCause = null; /// Reason for why the client disconnected
    const int m_allowedConnections = 100;      /// Maximum allowed connections to Photon Networking
    List<TypedLobby> m_lobbys = null;          /// Holds one lobby per game type
    LevelID m_levelJoined = LevelID.NO_LEVEL;  /// Level the player has joined
    GameObject m_player = null;                /// Main client player generated

    /// <summary>
    /// Initialises the Photon Networking Matchmaker
    /// </summary>
    void Start()
    {
        m_lobbys = (from i in Enumerable.Range(0, Utilities.GetMaxLevels())
                    select new TypedLobby()).ToList();

        PhotonNetwork.autoJoinLobby = false;

        if(!IsConnected())
        {
            ConnectToMatchmaker();
        }
    }

    /// <summary>
    /// Attempts to connect to Photon Networking
    /// </summary>
    private void ConnectToMatchmaker()
    {
        m_disconnectCause = null;
        PhotonNetwork.ConnectUsingSettings(Utilities.GameVersion());
    }

    /// <summary>
    /// Gets the status of the connection
    /// </summary>
    public string GetNetworkStatus()
    {
        return PhotonNetwork.connectionStateDetailed.ToString();
    }

    /// <summary>
    /// Gets whether a room is ready to start the game
    /// </summary>
    public bool IsRoomReady()
    {
        if(PhotonNetwork.inRoom)
        {
            Debug.Log("Max Players: " + PhotonNetwork.room.maxPlayers);
            Debug.Log("Player Count: " + PhotonNetwork.room.playerCount);
            return PhotonNetwork.room.playerCount >= PhotonNetwork.room.maxPlayers;
        }
        return false;
    }

    /// <summary>
    /// Gets the maximum number of players than can enter the room
    /// </summary>
    public int GetRoomMaxSlots()
    {
        return IsInRoom() ? PhotonNetwork.room.maxPlayers : 0;
    }

    /// <summary>
    /// Gets the number of players currently in the room
    /// </summary>
    public int GetRoomPlayerCount()
    {
        return IsInRoom() ? PhotonNetwork.room.playerCount : 0;
    }

    /// <summary>
    /// Gets whether the client is in a room
    /// </summary>
    public bool IsInRoom()
    {
        return PhotonNetwork.inRoom;
    }

    /// <summary>
    /// Gets whether the client is in a lobby
    /// </summary>
    private bool IsInLobby()
    {
        return PhotonNetwork.insideLobby;
    }

    /// <summary>
    /// Gets whether the client is connected to Photon Network
    /// </summary>
    public bool IsConnected()
    {
        return PhotonNetwork.connected;
    }
   
    /// <summary>
    /// Called when the client left a room
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("Client left room");

        if(IsInLobby())
        {
            PhotonNetwork.LeaveLobby();
        }
    }

    /// <summary>
    /// Called when a JoinRandom() call failed
    /// This is usually because a room does not yet exist or is full
    /// </summary>
    public void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Could not find room. Creating new room.");

        PhotonNetwork.CreateRoom(null, true, true,
            (byte)Utilities.GetAcceptedPlayersForLevel(m_levelJoined));
    }
    
    /// <summary>
    /// Called on entering a lobby on the Master Server
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Client joined lobby");

        PhotonNetwork.JoinRandomRoom(null, 
            (byte)Utilities.GetAcceptedPlayersForLevel(m_levelJoined));
    }
    
    /// <summary>
    /// Called after leaving a lobby
    /// </summary>
    public override void OnLeftLobby()
    {
        Debug.Log("Client left lobby");
        m_levelJoined = LevelID.NO_LEVEL;
    }
    
    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection
    /// was established, followed by a call to OnDisconnectedFromPhoton().
    /// </summary>
    /// <param name="cause">Reason why client failed to connect</param>
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        m_disconnectCause = cause;
    }
    
    /// <summary>
    /// Called after disconnecting from the Photon server
    /// </summary>
    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log(m_disconnectCause != null ? 
            m_disconnectCause.ToString() : "Disconnected");

        ConnectToMatchmaker();

        if(Utilities.IsLevelLoaded())
        {
            //TODO: Show disconnect message and go to the lobby
            Application.LoadLevel((int)SceneID.LOBBY);
        }
    }
   
    /// <summary>
    /// Player attempts to join a level
    /// </summary>
    public void JoinGameLevel(LevelID level)
    {
        Debug.Log("Attempting to join level: " + level.ToString());

        m_levelJoined = level;
        PhotonNetwork.JoinLobby(m_lobbys[(int)level]);
    }

    /// <summary>
    /// Leaves the level currently connected to
    /// </summary>
    public void LeaveGameLevel()
    {
        if(IsInRoom())
        {
            PhotonNetwork.LeaveRoom();
        }
        else if(IsInLobby())
        {
            PhotonNetwork.LeaveLobby();
        }
    }

    /// <summary>
    /// Called when entering a room (by creating or joining it)
    /// Called on all clients (including the Master Client)
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Client Joined room");
    }

    /// <summary>
    /// Called after the connection to the master is established and 
    /// authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // Allows joining a game without using the lobby
        if(Utilities.IsLevelLoaded() && !IsInRoom() && 
           m_levelJoined == LevelID.NO_LEVEL)
        {
            JoinGameLevel(Utilities.GetLoadedLevel());
        }
    }

    /// <summary>
    /// Destroys the player
    /// </summary>
    public void DestroyPlayer()
    {
        m_player.GetComponentInChildren<NetworkedPlayer>().UnInitialiseClient();
        m_player = null;
    }

    /// <summary>
    /// Creates a new player once the level has started
    /// </summary>
    void CreatePlayer()
    {
        m_player = PhotonNetwork.Instantiate(
            "PlayerPVP", Vector3.zero, Quaternion.identity, 0);
        
        m_player.GetComponentInChildren<NetworkedPlayer>().InitialiseClient();
    }

    /// <summary>
    /// Updates the network matchmaker
    /// </summary>
    void Update()
    {
        // Creates a new player when the level has fully initialised
        if(m_player == null && 
           Utilities.IsLevelLoaded() && 
           !Utilities.IsGameOver() && 
           IsInRoom())
        {
            CreatePlayer();
        }
    }

    /// <summary>
    /// Gets the Network Matchmaker instance from the scene
    /// </summary>
    public static NetworkMatchmaker Get()
    {
        var network = FindObjectOfType<NetworkMatchmaker>();
        if(network == null)
        {
            throw new NullReferenceException(
                "Could not find Random Matchmaker in scene");
        }
        return network;
    }
}