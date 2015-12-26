////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkMatchmaker.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls connecting to Photon Unity Networking for multiplayer play
/// </summary>
public class NetworkMatchmaker : Photon.PunBehaviour
{
    List<TypedLobby> m_lobbys = null;           /// Holds one lobby per game type
    LevelID m_levelJoined = LevelID.NO_LEVEL;   /// Current level the player has joined
    DisconnectCause? m_disconnectCause = null;  /// Why the client disconnected or null if connected
    float m_reconnectTimer = 0.0f;              /// Timer to count down for reconnection attempts
    string m_networkStatus = "";                /// Public description of the network connection
    string m_networkDiagnostic = "";            /// Diagnostic description of the network connection
    SynchedPrefabs m_synchedPrefabs = null;     /// Prefabs synched across networked players
        
    /// <summary>
    /// Initialises the matchmaker
    /// </summary>
    void Start()
    {
        PhotonNetwork.autoJoinLobby = false;

        m_synchedPrefabs = new SynchedPrefabs();

        int maxLevels = Utilities.GetMaxLevels();
        m_lobbys = (from i in Enumerable.Range(0, maxLevels)
                    select new TypedLobby()).ToList();

        if (!IsConnected())
        {
            SetStatus("Connecting to server");
            ConnectToMatchmaker();
        }
    }
   
    /// <summary>
    /// Gets the status description of the connection
    /// </summary>
    public string GetNetworkStatus()
    {
        return m_networkStatus;
    }

    /// <summary>
    /// Gets the networked time synched with the server
    /// </summary>
    public double GetTime()
    {
        return PhotonNetwork.time;
    }

    /// <summary>
    /// Gets whether the client is in a rooom which is filled with players
    /// </summary>
    public bool IsRoomReady()
    {
        if(PhotonNetwork.inRoom)
        {
            return Utilities.IsOpenLeveL(m_levelJoined) ||
                PhotonNetwork.room.playerCount >= PhotonNetwork.room.maxPlayers;
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
    /// Gets the unique player number assigned on connection
    /// </summary>
    public int GetPlayerID()
    {
        return PhotonNetwork.player.ID;
    }

    /// <summary>
    /// Gets the ID of the player as a zero-based index
    /// in comparison to other players in the list
    /// </summary>
    public int GetPlayerIndex()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        var playerIDs = (from player in players 
                         orderby player.ID ascending
                         select player.ID).ToList();

        return playerIDs.FindIndex(ID => ID == GetPlayerID());
    }

    /// <summary>
    /// Gets whether the client is in a room
    /// Note when the client is in a room they are not in a lobby
    /// </summary>
    public bool IsInRoom()
    {
        return PhotonNetwork.inRoom;
    }

    /// <summary>
    /// Gets whether the client is in a lobby
    /// Note when the client is in a lobby they are not in a room
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
    /// Gets whether the client is connected to a game level
    /// </summary>
    public bool IsConnectedToLevel()
    {
        return IsConnected() && IsInRoom();
    }

    /// <summary>
    /// Called when the client left a room
    /// </summary>
    public override void OnLeftRoom()
    {
        SetStatus("Left Level");

        if(IsInLobby())
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
        SetStatus("Joined Level");
    }

    /// <summary>
    /// Called on entering a lobby
    /// </summary>
    public override void OnJoinedLobby()
    {
        SetDiagnostic("Joined Lobby");

        PhotonNetwork.JoinRandomRoom(null, 
            (byte)Utilities.GetMaximumPlayers());
    }
    
    /// <summary>
    /// Called after leaving a lobby
    /// </summary>
    public override void OnLeftLobby()
    {
        SetDiagnostic("Left Lobby");
    }

    /// <summary>
    /// Called when a JoinRandom call failed
    /// This is because a room does not yet exist or is full
    /// </summary>
    public void OnPhotonRandomJoinFailed()
    {
        SetStatus("Creating new game");

        RoomOptions options = new RoomOptions();
        options.isOpen = true;
        options.isVisible = true;
        options.maxPlayers = (byte)Utilities.GetMaximumPlayers();

        PhotonNetwork.CreateRoom(null, options, m_lobbys[(int)m_levelJoined]);
    }
    
    /// <summary>
    /// Called if a connect call to the Photon server failed before the connection
    /// was established, followed by a call to OnDisconnectedFromPhoton
    /// </summary>
    /// <param name="cause">Reason why client failed to connect</param>
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        m_disconnectCause = cause;
    }

    /// <summary>
    /// Called when something causes the connection to fail (after it was established)
    /// </summary>
    /// <param name="cause">Reason why client failed to connect</param>
    public override void OnConnectionFail(DisconnectCause cause)
    {
        m_disconnectCause = cause;
    }

    /// <summary>
    /// Called after disconnecting from the Photon server
    /// </summary>
    public override void OnDisconnectedFromPhoton()
    {
        if(m_disconnectCause == null)
        {
            m_disconnectCause = DisconnectCause.Exception;
        }

        SetStatus("Disconnected from server\n(" +
            m_disconnectCause.ToString() +
            ")\nAttempting to reconnect");

        m_reconnectTimer = 1.0f;
    }

    /// <summary>
    /// Attempts to connect to Photon Unity Networking
    /// Will call OnFailedToConnectToPhoton if failed
    /// </summary>
    private void ConnectToMatchmaker()
    {
        PhotonNetwork.ConnectUsingSettings(Utilities.GameVersion());
    }

    /// <summary>
    /// Returns the cause of the player disconnecting
    /// </summary>
    public string GetDisconnectCause()
    {
        return m_disconnectCause.ToString();
    }
   
    /// <summary>
    /// Connects the player to the requested game level
    /// Will find the first open rooom with available slots
    /// </summary>
    public void JoinGameLevel(LevelID level)
    {
        SetStatus("Joining Level");

        m_levelJoined = level;
        PhotonNetwork.JoinLobby(m_lobbys[(int)level]);
    }

    /// <summary>
    /// Starts the level by closing the room if needed
    /// </summary>
    public void StartLevel()
    {
        if(IsInRoom() && !Utilities.IsOpenLeveL(m_levelJoined))
        {
            SetDiagnostic("Closing Level");
            PhotonNetwork.room.open = false;
        }
    }

    /// <summary>
    /// Leaves the level currently connected to
    /// </summary>
    public void LeaveGameLevel()
    {
        m_synchedPrefabs.Destroy();
       
        if(IsInRoom())
        {
            // Player is the last one left in room
            if(!Utilities.IsOpenLeveL(m_levelJoined) && 
               PhotonNetwork.room.playerCount == 1)
            {
                PhotonNetwork.room.open = true;
            }

            PhotonNetwork.LeaveRoom();
        }
        else if(IsInLobby())
        {
            PhotonNetwork.LeaveLobby();
        }
    }

    /// <summary>
    /// Called after the connection to the master is established and 
    /// authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        SetStatus("Connected to server");

        if(Utilities.IsLevelLoaded() && !IsConnectedToLevel())
        {
            // Level has not been joined using the lobby (for development)
            // Or level joined can be easily rejoined
            if(m_levelJoined == LevelID.NO_LEVEL ||
               Utilities.IsOpenLeveL(m_levelJoined))
            {
                JoinGameLevel(Utilities.GetLoadedLevel());
            }
            else
            {
                // Player has disconnected during a closed level
                // Force move the player back to the lobby
                m_levelJoined = LevelID.NO_LEVEL;
                Application.LoadLevel((int)SceneID.LOBBY);
            }
        }
    }

    /// <summary>
    /// Destroys the player and other prefabs on all connected clients
    /// </summary>
    public void DestroyPrefabs()
    {
        SetDiagnostic("Destroying client");
        m_synchedPrefabs.Destroy();
    }

    /// <summary>
    /// Creates a new player and other prefabs on all connected clients
    /// </summary>
    void CreatePlayer()
    {
        SetDiagnostic("Creating client");
        m_synchedPrefabs.Create();
    }

    /// <summary>
    /// Sets the network status and logs a debug message
    /// </summary>
    void SetStatus(string text)
    {
        m_networkStatus = text;
        SetDiagnostic(text);
    }

    /// <summary>
    /// Sets the network diagnostic status and logs a debug message
    /// </summary>
    void SetDiagnostic(string text)
    {
        m_networkDiagnostic = text;
        Debug.Log(text);
    }
        
    /// <summary>
    /// Updates the network matchmaker
    /// </summary>
    void Update()
    {
        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Network Status", m_networkDiagnostic);
            Diagnostics.Add("Disconnect Status", m_disconnectCause);
            Diagnostics.Add("Ping", PhotonNetwork.GetPing());
            Diagnostics.Add("Server Time", PhotonNetwork.time);
        }

        // Attempt to reconnect when disconnected
        if(m_reconnectTimer != 0.0f)
        {
            m_reconnectTimer -= Time.deltaTime;
            if(m_reconnectTimer <= 0.0f)
            {
                m_reconnectTimer = 0.0f;
                ConnectToMatchmaker();
            }
        }

        // Creates a new player when the level has fully initialised
        if(!m_synchedPrefabs.IsInitialised() && 
           IsConnectedToLevel() &&
           Utilities.IsLevelLoaded() && 
           !Utilities.IsGameOver())
        {
            m_synchedPrefabs.Create();
        }
    }

    /// <summary>
    /// Gets the Network Matchmaker instance from the scene
    /// Note, relies on only one matchmaker instance per scene
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