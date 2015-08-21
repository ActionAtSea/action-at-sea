////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Common.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/**
* Fade state
*/
enum FadeState
{
    FADE_IN,
    FADE_OUT,
    NO_FADE
};

/**
* Scene IDS
*/
enum SceneID
{
    MENU = 0,
    MOVE_AND_FIRE = 1,
    DISCOVER_LAND = 2,
    UPGRADING_DOCKS = 3,
    ENEMIES = 4,
    TREASURE = 5,
    GAME = 6
}