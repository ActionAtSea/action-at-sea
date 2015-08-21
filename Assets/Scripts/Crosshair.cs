////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Crosshair.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public bool hideCursor = false;

    /**
    * Updates the cursor
    */
    void Update()
    {
        HideCursor();

        //Attached object (the crosshair) matches the position of the mouse
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f;

        //converts the screen space postion of the mouse to relative game world position
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        gameObject.transform.position = mousePos;
    }

    /**
    * Hides the cursor
    */
    private void HideCursor()
    {
        if (hideCursor)
        {
            Cursor.visible = false;
        }
    }

    /**
    * Shows the cursor
    */
    public void ShowCursor()
    {
        Cursor.visible = true;
        hideCursor = false;
    }
}