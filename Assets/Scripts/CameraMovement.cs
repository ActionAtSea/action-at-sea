////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CameraMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    public float dragAmount = 0.15f;
    public bool useCameraDrag = true;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_fixedPosition;
    private Vector3 m_viewportPosition;
    private GameObject m_target = null;

    /**
    * Initialises the script
    */
    void Start ()
    {
        m_viewportPosition = new Vector3 (0.5f, 0.5f, 0.0f);
        m_fixedPosition = new Vector3 (0.0f, 0.0f, transform.position.z);
    }

    /**
    * Updates the dragged camera once a player has been found
    */
    void FixedUpdate ()
    {
        if(m_target == null)
        {
            m_target = GamePlayers.GetControllablePlayer();
            return;
        }

        if(useCameraDrag)
        {
            // Reference: http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html
            var camera = GetComponent<Camera> ();
            m_viewportPosition.z = camera.WorldToViewportPoint(m_target.transform.position).z;
            Vector3 delta = m_target.transform.position - camera.ViewportToWorldPoint(m_viewportPosition);
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref m_velocity, dragAmount);
        }
        else
        {
            m_fixedPosition.x = m_target.transform.position.x;
            m_fixedPosition.y = m_target.transform.position.y;
            transform.position = m_fixedPosition;
        }
    }
}
