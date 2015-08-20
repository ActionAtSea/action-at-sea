////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GameInformation.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour 
{
    public string playerName = "Unnamed";	
    static bool sm_isInitialised = false;
    static string sm_playerName = "Unnamed";
	bool m_initialised = false;
    
    void Start () 
    {
        if(!sm_isInitialised)
        {
            sm_isInitialised = true;
            sm_playerName = playerName;
        }
    }

	void Update()
	{
		if(!m_initialised)
		{
			m_initialised = true;

			if(Application.loadedLevel == (int)SceneID.GAME)
			{
				SoundManager.Get().PlayMusic(SoundManager.MusicID.GAME_TRACK);
				SoundManager.Get().PlayMusic(SoundManager.MusicID.GAME_AMBIENCE);
			}
			else
			{
				SoundManager.Get().PlayMusic(SoundManager.MusicID.MENU_TRACK);
			}
		}
	}

    static public void SetPlayerName(string name)
    {
        sm_playerName = name;
    }

    static public string GetPlayerName()
    {
        return sm_playerName;
    }
}
