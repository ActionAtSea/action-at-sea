////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Crosshair.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    private Vector3 mousePos;
    public bool hideCursor = false;
	
    void Update()
    {
        HideCursor();

        //Attached object (the crosshair) matches the position of the mouse
        mousePos = Input.mousePosition;
        mousePos.z = 10.0f;

        //converts the screen space postion of the mouse to relative game world position
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        gameObject.transform.position = mousePos;
    }

    private void HideCursor()
    {
        if (hideCursor)
        {
            Cursor.visible = false;
        }
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        hideCursor = false;
    }
}