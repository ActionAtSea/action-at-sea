////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Crosshair.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public bool hideCursor = false;

    /// <summary>
    /// Updates the cursor
    /// </summary>
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

    /// <summary>
    /// Hides the cursor
    /// </summary>
    private void HideCursor()
    {
        if (hideCursor)
        {
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Shows the cursor
    /// </summary>
    public void ShowCursor()
    {
        Cursor.visible = true;
        hideCursor = false;
    }

    /// <summary>
    /// Gets the Cursor from the scene
    /// </summary>
    public static Crosshair Get()
    {
        var cursor = FindObjectOfType<Crosshair>();
        if(cursor == null)
        {
            Debug.LogError("Could not find Cursor");
        }
        return cursor;
    }
}