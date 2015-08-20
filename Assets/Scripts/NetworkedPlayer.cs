using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{	
	static int playerIDCounter = 0;
	private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	public PhotonView photonView = null;
	public string PlayerName = "unnamed";
	public string PlayerID;
	public int PlayerScore = 0;
	private float HealthLevel = -1.0f;
	private bool connected = false;

	void Update()
	{
		if(PlayerID == "")
		{
			if(!photonView.isMine)
			{
				playerIDCounter++;
				PlayerID = "Enemy" + playerIDCounter.ToString();
			}
			else
			{
				PlayerID = "Player";
			}
		}

		if (!photonView.isMine)
		{
			if(connected)
			{
				transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
				transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);

				if(HealthLevel >= 0)
				{
					GetComponent<Health>().SetHealthLevel(HealthLevel);
					if(!GetComponent<Health>().IsAlive)
					{
						FindObjectOfType<AnimationGenerator>().PlaceExplosion(transform.position);
						Destroy(transform.parent.gameObject);
		            }
				}
			}
		}
		else
		{
			PlayerScore = (int)GetComponent<PlayerScore>().RoundedScore;
			PlayerName = GameInformation.GetPlayerName();
			HealthLevel = GetComponent<Health>().HealthLevel;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			connected = true;
			stream.SendNext(connected);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(HealthLevel);
			stream.SendNext(PlayerName);
			stream.SendNext(PlayerScore);
		}
		else
		{
			// Network player, receive data
			connected = (bool)stream.ReceiveNext();
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
			HealthLevel = (float)stream.ReceiveNext();
			PlayerName = (string)stream.ReceiveNext();
			PlayerScore = (int)stream.ReceiveNext();
		}
	}
}
