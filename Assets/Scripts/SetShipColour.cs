using UnityEngine;
using System.Collections;

public class SetShipColour : MonoBehaviour
{
	public void SetColour(Color color)
    {
        GetComponent<MeshRenderer>().material.SetColor("_TrimmingColour", color);

        for(int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
            if(child != null)
            {
                child.material.SetColor("_TrimmingColour", color);
            }
        }

    }
}
