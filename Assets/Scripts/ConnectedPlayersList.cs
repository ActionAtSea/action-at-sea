using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectedPlayersList : MonoBehaviour
{
    UnityEngine.UI.Text m_text = null;

    void Start()
    {
        m_text = GetComponent<UnityEngine.UI.Text>();
        m_text.text = "";
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }
}
