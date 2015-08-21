﻿////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Cannon.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
    public PhotonView photonView = null;
    public bool rightSideCannon = true;  // Determines which side the cannon is on the ship.

    private Vector3 m_firePosition = new Vector3();
    private Quaternion m_fireRotation = new Quaternion();
    private bool m_hasFired = false;
    private bool m_shouldFire = false;
    private float m_swivelRangeDegrees = 45.0f;  // The range that the cannons can swivel.
    private float m_cursorAngle = 0.0f;          // stores the angle the mouse cursor is at relative to the ship.
    private BulletFireScript m_fireScript = null;
    private CannonController m_controller = null;

    /**
    * Initialises the script
    */
    void Start()
    {
        m_fireScript = GetComponent<BulletFireScript>();
        m_controller = GetComponentInParent<CannonController>();
        m_swivelRangeDegrees = m_controller.SwivelRangeDegrees;
    }

    /**
    * Updates the cannon
    */
    void Update()
    {
        if(m_controller.controllable)
        {
            m_firePosition = m_fireScript.FirePosition();
            m_fireRotation = m_fireScript.FireRotation();
            UpdateRotation();
        }
        
        if(m_shouldFire)
        {
            FireGun();
            m_shouldFire = false;
        }
    }

    /*
    * TODO:
    * Set the cannon's firing angle to their limits closest to the mouse cursor
    * angle when the mouse cursor move from the one side of the ship to the other
    * and is outside of the cannon tracking range.
    */
    private void UpdateRotation()
    {
        m_cursorAngle = m_controller.MouseCursorAngle;

        if (rightSideCannon)
        {
            if (m_cursorAngle <= (0.0f + m_swivelRangeDegrees) && m_cursorAngle >= 0.0f)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, m_cursorAngle);
            }
            else if (m_cursorAngle >= (360.0f - m_swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, m_cursorAngle);
            }
        }
        else
        {
            if (m_cursorAngle >= (180.0f - m_swivelRangeDegrees) && m_cursorAngle <= (180.0f + m_swivelRangeDegrees))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, m_cursorAngle);
            }
        }
    }

    /**
    * Serialises the cannon across the network
    */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.localEulerAngles);
            stream.SendNext(m_hasFired);
            stream.SendNext(m_firePosition);
            stream.SendNext(m_fireRotation);
            m_hasFired = false;
        }
        else
        {
            var angles = (Vector3)stream.ReceiveNext();
            transform.localEulerAngles = new Vector3(angles.x, angles.y, angles.z);
            m_shouldFire = (bool)stream.ReceiveNext();
            m_firePosition = (Vector3)stream.ReceiveNext();
            m_fireRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    /**
    * Fires the cannon
    */
    public void FireGun()
    {
        string ID = transform.parent.transform.parent.GetComponent<NetworkedPlayer>().PlayerID;
        m_fireScript.Fire(ID, m_firePosition, m_fireRotation);
        m_hasFired = m_controller.controllable;
    }
}