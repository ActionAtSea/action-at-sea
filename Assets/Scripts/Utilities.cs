////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Utilities.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// State of fading
/// </summary>
public enum FadeState
{
    FADE_IN,
    FADE_OUT,
    NO_FADE
};

/// <summary>
/// IDs for each application scene
/// Note, adding a new level requires updating the methods below
/// </summary>
public enum SceneID
{
    MENU = 0,
    MOVE_AND_FIRE = 1,
    DISCOVER_LAND = 2,
    UPGRADING_DOCKS = 3,
    ENEMIES = 4,
    TREASURE = 5,
    LEVEL1 = 6,
    LEVEL2 = 7,
    LOBBY = 8,
    LEVEL3 = 9,
    LEVEL4 = 10,
    LEVEL5 = 11,
    MAX_SCENES
}

/// <summary>
/// IDs for each level
/// </summary>
public enum LevelID
{
    NO_LEVEL = -1,
    LEVEL1 = 0,
    LEVEL2,
    LEVEL3,
    LEVEL4,
    LEVEL5,
    MAX_LEVELS
}

/// <summary>
/// States that a level can be in
/// Relies on incrementing during a level
/// </summary>  
public enum GameState
{
    NONE,
    STAGE_1,
    STAGE_2
}

class Utilities
{
    static string sm_defaultName = "Unnamed";
    static string sm_playerName = sm_defaultName;
    static int sm_maxPlayers = 0;

    /// <summary>
    /// Returns the game version
    /// </summary>
    static public string GameVersion()
    {
        return "0.1";
    }

    /// <summary>
    /// Sets the player name
    /// </summary>
    static public void SetPlayerName(string name)
    {
        sm_playerName = name;
    }

    /// <summary>
    /// Gets the player default name
    /// </summary>
    static public string GetPlayerDefaultName()
    {
        return sm_defaultName;

    }

    /// <summary>
    /// Gets the player name
    /// </summary>
    static public string GetPlayerName()
    {
        return sm_playerName;
    }

    /// <summary>
    /// Returns whether currently game over
    /// </summary>
    static public bool IsGameOver()
    {
        return GameOverScript.Get().IsGameOver();
    }

    /// <summary>
    /// Returns the currently loaded level
    /// </summary>
    static public LevelID GetLoadedLevel()
    {
        return GetLevelFromSceneID(Application.loadedLevel);
    }

    /// <summary>
    /// Returns whether a level is currently loaded
    /// </summary>
    static public bool IsLevelLoaded()
    {
        return Application.loadedLevel == (int)SceneID.LEVEL1 ||
               Application.loadedLevel == (int)SceneID.LEVEL2 ||
               Application.loadedLevel == (int)SceneID.LEVEL3 ||
               Application.loadedLevel == (int)SceneID.LEVEL4 ||
               Application.loadedLevel == (int)SceneID.LEVEL5;
    }

    /// <summary>
    /// Returns the state of the game
    /// </summary>
    static public GameState GetGameState()
    {
        return GameModeManager.Get().GetState();
    }

    /// <summary>
    /// Returns the current number of levels supported
    /// </summary>
    static public int GetMaxLevels()
    {
        return (int)LevelID.MAX_LEVELS;
    }

    /// <summary>
    /// Returns the screen ID for the level
    /// </summary>
    static public SceneID GetSceneIDFromLevel(LevelID level)
    {
        switch(level)
        {
        case LevelID.LEVEL1:
            return SceneID.LEVEL1;
        case LevelID.LEVEL2:
            return SceneID.LEVEL2;
        case LevelID.LEVEL3:
            return SceneID.LEVEL3;
        case LevelID.LEVEL4:
            return SceneID.LEVEL4;
        case LevelID.LEVEL5:
            return SceneID.LEVEL5;
        default:
            throw new ArgumentException("Unknown level ID");
        }
    }

    /// <summary>
    /// Returns the number of AI for the level
    /// </summary>
    static public int GetAICount()
    {
        switch (Utilities.GetLoadedLevel())
        {
            case LevelID.LEVEL1:
                return 0;
            case LevelID.LEVEL2:
                return 4;
            case LevelID.LEVEL3:
                return 0;
            case LevelID.LEVEL4:
                return 0;
            case LevelID.LEVEL5:
                return 0;
            default:
                throw new ArgumentException("Unknown level ID");
        }
    }

    /// <summary>
    /// Returns the name for the level
    /// </summary>
    static public string GetLevelName(LevelID level)
    {
        switch (level)
        {
            case LevelID.LEVEL1:
                return "Free For All";
            case LevelID.LEVEL2:
                return "Level 2";
            case LevelID.LEVEL3:
                return "Level 3";
            case LevelID.LEVEL4:
                return "Level 4";
            case LevelID.LEVEL5:
                return "Level 5";
            default:
                throw new ArgumentException("Unknown level ID");
        }
    }

    /// <summary>
    /// Returns the current level
    /// </summary>
    static public LevelID GetLevelFromSceneID(int sceneID)
    {
        switch(sceneID)
        {
        case (int)SceneID.LEVEL1:
            return LevelID.LEVEL1;
        case (int)SceneID.LEVEL2:
            return LevelID.LEVEL2;
        case (int)SceneID.LEVEL3:
            return LevelID.LEVEL3;
        case (int)SceneID.LEVEL4:
            return LevelID.LEVEL4;
        case (int)SceneID.LEVEL5:
            return LevelID.LEVEL5;
        default:
            throw new ArgumentException("loaded scene not a level");
        }
    }

    /// <summary>
    /// Returns whether the level is an open level
    /// This means anyone can connect/disconnect at any time
    /// </summary>
    static public bool IsOpenLeveL(LevelID level)
    {
        return level == LevelID.LEVEL1;
    }

    /// <summary>
    /// Returns whether the level is an open level
    /// This means anyone can connect/disconnect at any time
    /// </summary>
    static public bool IsOpenLeveL()
    {
        return IsOpenLeveL(GetLoadedLevel());
    }

    /// <summary>
    /// Returns how many players are allowed the selected level
    /// </summary>
    static public int GetMaximumPlayers()
    {
        return sm_maxPlayers;
    }

    /// <summary>
    /// Returns how many players are allowed in each level
    /// </summary>
    static public void SetMaximumPlayers(int value)
    {
        sm_maxPlayers = value;
    }

    /// <summary>
    /// Returns how many players are allowed in each level
    /// </summary>
    static public int GetMaxPlayersForLevel(LevelID level)
    {
        switch(level)
        {
        case LevelID.LEVEL1:
            return 20;
        case LevelID.LEVEL2:
            return 8;
        case LevelID.LEVEL3:
            return 8;
        case LevelID.LEVEL4:
            return 8;
        case LevelID.LEVEL5:
            return 8;
        default:
             throw new ArgumentException("Unknown level ID");
        }
    }

    /// <summary>
    /// Gets an ordered list of the object type
    /// Ordering by name is important for networking
    /// </summary>
    static public List<GameObject> GetOrderedList(string tag)
    {
        return GetOrderedList<GameObject>(GameObject.FindGameObjectsWithTag(tag));
    }

    /// <summary>
    /// Gets an ordered list of the object type
    /// Ordering by name is important for networking
    /// </summary>
    static public List<T> GetOrderedList<T>() where T : UnityEngine.Object
    {
        return GetOrderedList<T>(GameObject.FindObjectsOfType<T>());
    }

    /// <summary>
    /// Gets an ordered list of the object type
    /// Ordering by name is important for networking
    /// </summary>
    static public List<T> GetOrderedListInChildren<T>(GameObject obj) where T : UnityEngine.Object
    {
        return GetOrderedList<T>(obj.GetComponentsInChildren<T>());
    }

    /// <summary>
    /// Gets an ordered list of the object type
    /// Ordering by name is important for networking
    /// </summary>
    static private List<T> GetOrderedList<T>(T[] objs) where T : UnityEngine.Object
    {
        List<T> orderedList = (from obj in objs 
                               orderby obj.name ascending
                               select obj).ToList();
        
        // Check to make sure all have a different name
        HashSet<string> names = new HashSet<string>();
        foreach(var obj in orderedList)
        {
            if(!names.Add(obj.name))
            {
                Debug.LogError(obj.name + " already exists! " +
                    "This error can cause many seemingly unrelated bugs. " +
                    "Ensure all islands have a seperate name.");
            }
        }

        return orderedList;
    }

    /// <summary>
    /// Whether the player is fully initialised
    /// </summary>
    static public bool IsPlayerInitialised(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedEntity>().IsInitialised();
    }
    
    /// <summary>
    /// Returns the player Name
    /// </summary>
    static public string GetPlayerName(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedEntity>().PlayerName : "";
    }
    
    /// <summary>
    /// Returns the player Score
    /// </summary>
    static public float GetPlayerScore(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedPlayer>().PlayerScore : 0.0f;
    }
    
    /// <summary>
    /// Returns the player color
    /// </summary>
    static public Color GetPlayerColor(GameObject obj)
    {
        return obj != null ? obj.GetComponentInParent<NetworkedEntity>().PlayerColor : new Color();
    }
    
    /// <summary>
    /// Returns the player ID
    /// </summary>
    static public int GetPlayerID(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedEntity>().PlayerID;
    }

    /// <summary>
    /// Returns whether the player can control this
    /// </summary>
    static public bool IsPlayerAlive(GameObject obj)
    {
        return obj.GetComponentInParent<Health>().IsAlive;
    }

    /// <summary>
    /// Returns whether the AI is assigned to the player
    /// </summary>
    static public bool IsAssignedAI(GameObject obj)
    {
        var entity = obj.GetComponentInParent<NetworkedEntity>();
        return entity.IsControllable() && entity.IsInitialised() && entity.name != "Rogue";
    }

    /// <summary>
    /// Returns whether the AI is controlled by the client
    /// </summary>
    static public bool IsControllableAI(GameObject obj)
    {
        var entity = obj.GetComponentInParent<NetworkedEntity>();
        return entity.IsControllable() && entity.IsInitialised();
    }

    /// <summary>
    /// Returns whether the player can control this
    /// </summary>
    static public bool IsPlayerControllable(GameObject obj)
    {
        return obj.GetComponentInParent<NetworkedEntity>().IsControllable();
    }
}