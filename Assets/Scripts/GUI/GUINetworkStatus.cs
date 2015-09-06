////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUINetworkStatus.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUINetworkStatus : MonoBehaviour
{
    private NetworkMatchmaker m_network = null;

    /// <summary>
    /// Initialises the script
    /// </summary>
    void Start()
    {
        m_network = NetworkMatchmaker.Get();
    }

    /// <summary>
    /// Updates the script
    /// </summary>
    void Update() 
    {
        if(!m_network.IsConnectedToLevel())
        {
            GetComponent<UnityEngine.UI.Text>().enabled = true;
            GetComponent<UnityEngine.UI.Text>().text = m_network.GetNetworkStatus();
        }
        else
        {
            GetComponent<UnityEngine.UI.Text>().enabled = false;
        }
    }
}