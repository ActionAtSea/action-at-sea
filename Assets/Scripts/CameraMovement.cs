////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CameraMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    private float m_dragAmount = 1.0f;
    private bool m_useCameraDrag = true;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_fixedPosition;
    private Vector3 m_viewportPosition;

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
        var player = PlayerManager.GetControllablePlayer();
        if(player == null)
        {
            return;
        }

        if(m_useCameraDrag)
        {
            // Reference: http://answers.unity3d.com/questions/29183/2d-camera-smooth-follow.html
            var camera = GetComponent<Camera> ();
            m_viewportPosition.z = camera.WorldToViewportPoint(player.transform.position).z;
            Vector3 delta = player.transform.position - camera.ViewportToWorldPoint(m_viewportPosition);
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref m_velocity, m_dragAmount);
        }
        else
        {
            m_fixedPosition.x = player.transform.position.x;
            m_fixedPosition.y = player.transform.position.y;
            transform.position = m_fixedPosition;
        }
    }
}
