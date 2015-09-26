////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - Crosshair.cs
////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    public Canvas parentCanvas;
    private bool shouldHide = true;

    /// <summary>
    /// Starts by hiding the cursor
    /// </summary>
    void Start()
    {
        Cursor.visible = false;
        shouldHide = true;
    }

    /// <summary>
    /// Updates the cursor
    /// </summary>
    void Update()
    {
        var canvasTransform = parentCanvas.GetComponent<RectTransform>();

        Cursor.visible = !shouldHide;
        transform.localPosition = new Vector3(
            Input.mousePosition.x - canvasTransform.sizeDelta.x / 2,
            Input.mousePosition.y - canvasTransform.sizeDelta.y / 2,
            0.0f);
    }

    /// <summary>
    /// Hides the cursor
    /// </summary>
    private void HideCursor()
    {
        GetComponent<UnityEngine.UI.Image>().enabled = true;
        shouldHide = true;
    }

    /// <summary>
    /// Shows the cursor
    /// </summary>
    public void ShowCursor()
    {
        GetComponent<UnityEngine.UI.Image>().enabled = false;
        shouldHide = false;
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