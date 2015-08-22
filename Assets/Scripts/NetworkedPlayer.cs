////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - NetworkedPlayer.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class NetworkedPlayer : MonoBehaviour 
{    
    public PhotonView photonView = null;
    public string PlayerName = "unnamed";
    public string PlayerID;
    public int PlayerScore = 0;

    static int sm_playerIDCounter = 0;
    private Vector3 m_correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion m_correctPlayerRot = Quaternion.identity; // We lerp towards this
    private float m_healthLevel = -1.0f;
    private bool m_connected = false;

    /**
    * Updates the player from the networked data
    */
    void Update()
    {
        if(PlayerID == "")
        {
            if(!photonView.isMine)
            {
                sm_playerIDCounter++;
                PlayerID = "Enemy" + sm_playerIDCounter.ToString();
            }
            else
            {
                PlayerID = "Player";
            }
        }

        if (!photonView.isMine)
        {
            if(m_connected)
            {
                transform.position = Vector3.Lerp(transform.position, m_correctPlayerPos, Time.deltaTime * 5);
                transform.rotation = Quaternion.Lerp(transform.rotation, m_correctPlayerRot, Time.deltaTime * 5);

                if(m_healthLevel >= 0)
                {
                    GetComponent<Health>().SetHealthLevel(m_healthLevel);
                    if(!GetComponent<Health>().IsAlive)
                    {
                        AnimationGenerator.Get().PlayAnimation(
                            transform.position, AnimationGenerator.ID.EXPLOSION);

                        Destroy(transform.parent.gameObject);
                    }
                }
            }
        }
        else
        {
            PlayerScore = (int)GetComponent<PlayerScore>().RoundedScore;
            PlayerName = GameInformation.GetPlayerName();
            m_healthLevel = GetComponent<Health>().HealthLevel;
        }
    }

    /**
    * Serialises player data to each player
    */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            m_connected = true;
            stream.SendNext(m_connected);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_healthLevel);
            stream.SendNext(PlayerName);
            stream.SendNext(PlayerScore);
        }
        else
        {
            // Network player, receive data
            m_connected = (bool)stream.ReceiveNext();
            m_correctPlayerPos = (Vector3)stream.ReceiveNext();
            m_correctPlayerRot = (Quaternion)stream.ReceiveNext();
            m_healthLevel = (float)stream.ReceiveNext();
            PlayerName = (string)stream.ReceiveNext();
            PlayerScore = (int)stream.ReceiveNext();
        }
    }
}
