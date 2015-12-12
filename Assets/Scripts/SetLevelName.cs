using UnityEngine;
using System.Collections;

public class SetLevelName : MonoBehaviour
{
    public int level = -1;

	void Start ()
    {
        if(level == -1)
        {
            Debug.LogError("Level ID Must be set!");
        }
        GetComponentInChildren<UnityEngine.UI.Text>().text = 
            Utilities.GetLevelName((LevelID)(level-1));
	}
}
