////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Common.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// State of fading
/// </summary>
enum FadeState
{
    FADE_IN,
    FADE_OUT,
    NO_FADE
};

/// <summary>
/// IDs for each application scene
/// </summary>
enum SceneID
{
    MENU = 0,
    MOVE_AND_FIRE = 1,
    DISCOVER_LAND = 2,
    UPGRADING_DOCKS = 3,
    ENEMIES = 4,
    TREASURE = 5,
    LEVEL1 = 6,
    LOBBY = 7,
    LEVEL2 = 8
}

class Utilities
{
    /// <summary>
    /// Returns the game version
    /// </summary>
    static public string GameVersion()
    {
        return "0.2";
    }

    /// <summary>
    /// Returns whether a level is currently loaded
    /// </summary>
    static public bool IsLevelLoaded()
    {
        return Application.loadedLevel == (int)SceneID.LEVEL1 ||
               Application.loadedLevel == (int)SceneID.LEVEL2;
    }

    /// <summary>
    /// Returns the currently loaded level
    /// </summary>
    static public int GetLoadedLevel()
    {
        return GetLevelFromSceneID(Application.loadedLevel);
    }

    /// <summary>
    /// Returns the screen ID for the level
    /// </summary>
    static public SceneID GetSceneIDFromLevel(int level)
    {
        switch(level)
        {
        case 1:
            return SceneID.LEVEL1;
        case 2:
            return SceneID.LEVEL2;
        default:
            throw new ArgumentException("Unknown level ID");
        }
    }

    /// <summary>
    /// Returns the current level
    /// </summary>
    static public int GetLevelFromSceneID(int sceneID)
    {
        switch(sceneID)
        {
        case (int)SceneID.LEVEL1:
            return 1;
        case (int)SceneID.LEVEL2:
            return 2;
        default:
            throw new ArgumentException("loaded scene not a level");
        }
    }

    /// <summary>
    /// Returns how many players are allowed in each level
    /// </summary>
    static public int GetAcceptedPlayersForLevel(int level)
    {
        switch(level)
        {
        case 1:
            return 0; //Unlimited
        case 2:
            return 2;
        default:
             throw new ArgumentException("Unknown level ID");
        }
    }
}