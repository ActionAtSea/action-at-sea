using UnityEngine;
using System.Collections;

public class SetShipColour : MonoBehaviour
{
	public void SetColour(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
