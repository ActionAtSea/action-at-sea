using UnityEngine;
using System.Collections;

public class LevelSwitcher : MonoBehaviour {

	[System.Serializable]
	public class LevelCombination {
		public string levelName;
		public int levelId;
	}
	
	private string[] levelNames;

	public LevelCombination[] levels;
	
	void Start() {
		levelNames = new string[levels.Length];
		int i = 0;
		foreach(LevelCombination lc in levels) {
			levelNames[i] = lc.levelName;
			i++;
		}
	}

	void OnGUI() {
		int newLevel = GUI.SelectionGrid(new Rect(Screen.width/2f-200,Screen.height-100,400,50),Application.loadedLevel,levelNames,4);
		if (newLevel != Application.loadedLevel) {
			Application.LoadLevel(levels[newLevel].levelId);
		}
	}
}
