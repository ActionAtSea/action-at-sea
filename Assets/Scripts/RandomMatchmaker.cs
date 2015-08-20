using UnityEngine;

public class RandomMatchmaker : Photon.PunBehaviour
{
	bool m_showGUI = true;

	void Start()
	{
	#if DEBUG
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
	#endif

		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	void Update()
	{
	}

	void OnGUI()
	{
		if(m_showGUI)
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("Joined lobby");
		PhotonNetwork.JoinRandomRoom();
	}

	public void OnPhotonRandomJoinFailed()
	{
		Debug.Log("Could not find room, creating new room");
		PhotonNetwork.CreateRoom(null);
	}
		
	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room");
		GameObject playerPVP = PhotonNetwork.Instantiate("PlayerPVP", Vector3.zero, Quaternion.identity, 0);

		GameObject player = playerPVP.transform.FindChild("Player").gameObject;
		player.tag = "Player";
		player.GetComponent<NetworkedPlayer>().PlayerID = "Player";
		player.GetComponent<PlayerAiming>().controllable = true;
		player.GetComponent<PlayerMovement>().controllable = true;
		player.GetComponent<Health>().controllable = true;
		player.transform.FindChild("Cannons").gameObject.GetComponent<CannonController>().controllable = true;
		player.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
		player.transform.position = FindObjectOfType<PlayerPlacer>().GetNewPosition();

		GameObject healthbar = playerPVP.transform.FindChild("FloatingHealthBar").gameObject;
		healthbar.SetActive(false);
	
		m_showGUI = false;
	}
}