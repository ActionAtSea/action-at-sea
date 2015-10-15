////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - CameraMovement.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    private float m_dragAmount = 0.3f;
    private bool m_useCameraDrag = true;
    private Vector3 m_velocity = Vector3.zero;
    private Vector3 m_fixedPosition;


    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start ()
    {
        m_fixedPosition = new Vector3 (0.0f, transform.position.y, 0.0f);
    }

    /// <summary>
    /// Updates the dragged camera once a player has been found
    /// </summary>
    /// 
    void CameraZoom()
    {
        if(Input.mouseScrollDelta.y < 0.0f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y +1.0f, transform.position.z);
        }
        else if(Input.mouseScrollDelta.y > 0.0f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y -1.0f, transform.position.z);
        }
    }

    void Update()
    {
        CameraZoom();
    }

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
            var camera = GetComponent<Camera>();
            Vector3 point = camera.WorldToViewportPoint(player.transform.position);
            Vector3 delta = player.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref m_velocity, m_dragAmount);
        }
        else
        {
            m_fixedPosition.x = player.transform.position.x;
            m_fixedPosition.z = player.transform.position.z;
            transform.position = m_fixedPosition;
        }
    }
}
