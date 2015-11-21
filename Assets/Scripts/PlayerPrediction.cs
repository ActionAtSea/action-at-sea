////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - PlayerPrediction.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Predicts the position/rotation of the player over a network
/// Reference: Game Engine Gems ch18 - Dead Reckoning for Networked Games
/// </summary>
public class PlayerPrediction
{    
    Vector3 m_lastKnownPosition;
    Vector3 m_currentNetworkedPosition;
    Vector3 m_position;

    Quaternion m_lastKnownRotation;
    Quaternion m_currentNetworkedRotation;
    Quaternion m_rotation;

    Vector3 m_currentNetworkedVelocity;
    Vector3 m_lastKnownVelocity;
    Vector3 m_velocity;

    float m_secSinceLastUpdateSent = 0.0f;
    float m_previousLastUpdateSecs = 0.0f;
    bool m_recievedNetworkUpdate = false;
    bool m_firstRecieved = false;

    public void OnNetworkUpdate(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        if(!m_firstRecieved)
        {
            m_lastKnownPosition = position;
            m_lastKnownRotation = rotation;
            m_position = position;
            m_rotation = rotation;
            m_firstRecieved = true;
        }
        else
        {
            m_lastKnownPosition = m_position;
            m_lastKnownRotation = m_rotation;
            m_lastKnownVelocity = m_velocity;
        }
        
        m_currentNetworkedPosition = position;
        m_currentNetworkedRotation = rotation;
        m_currentNetworkedVelocity = velocity;

        m_recievedNetworkUpdate = true;
        m_previousLastUpdateSecs = m_secSinceLastUpdateSent;
        m_secSinceLastUpdateSent = 0.0f;
    }

    public void Update()
    {
        m_secSinceLastUpdateSent += Time.deltaTime;

        if(Diagnostics.IsActive())
        {
            Diagnostics.Add("Network Update Time", m_secSinceLastUpdateSent);
        }

        // For now lerp rotation
        m_rotation = Quaternion.Lerp(
            m_rotation, m_currentNetworkedRotation, Time.deltaTime * 5);

        if(m_recievedNetworkUpdate)
        {
            // ToDo: Velocity Blending
            m_position = m_currentNetworkedPosition;
            m_recievedNetworkUpdate = false;
        }
        else
        {
            m_position = m_position + (m_currentNetworkedVelocity * m_previousLastUpdateSecs);
        }
    }
        
    public Vector3 GetPosition()
    {
        return m_position;
    }

    public Quaternion GetRotation()
    {
        return m_rotation;
    }
}
