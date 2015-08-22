////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - GUINetworkStatus.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GUINetworkStatus : MonoBehaviour
{
    private bool m_joined = false;

    void Update() 
    {
        if(!m_joined)
        {
            var network = RandomMatchmaker.Get();
            m_joined = network.IsConnected();

            GetComponent<UnityEngine.UI.Text>().text = 
                network.GetNetworkStatus();

            if(m_joined)
            {
                GetComponent<UnityEngine.UI.Text>().enabled = false;
            }
        }
    }
}