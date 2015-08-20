using UnityEngine;
using System.Collections;

public class GetPlayerName : MonoBehaviour {
	
	void Start () 
	{
		GetComponent<UnityEngine.UI.Text>().text = 
			GameInformation.GetPlayerName();
	}
}
