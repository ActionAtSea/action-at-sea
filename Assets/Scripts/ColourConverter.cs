////////////////////////////////////////////////////////////////////////////////////////
// Action At Sea - ColourConverter.cs
////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class Colour
{
    /// <summary>
    /// Gets whether the colour has saturation and value
    /// Note, discards value and saturation
    /// http://www.poynton.com/PDFs/coloureq.pdf
    /// </summary>
    static public bool HasSaturationValue(Color color)
    {
        float max = Mathf.Max(Mathf.Max(color.r, color.b), color.g);
        float min = Mathf.Min(Mathf.Min(color.r, color.b), color.g);
        return (max - min) / max != 1.0f || max != 1.0f;
    }

    /// <summary>
    /// Gets the hue from an rgb colour
    /// Note, discards value and saturation
    /// http://www.poynton.com/PDFs/coloureq.pdf
    /// </summary>
    static public int RGBToHue(Color color)
    {
        // Return no hue if the colour is white
        if(color.r == 1.0f && color.g == 1.0f && color.b == 1.0f)
        {
            return -1;
        }

        float max = Mathf.Max(Mathf.Max(color.r, color.b), color.g);
        float min = Mathf.Min(Mathf.Min(color.r, color.b), color.g);
        float difference = max - min;
        float r = (max - color.r) / difference;
        float g = (max - color.g) / difference;
        float b = (max - color.b) / difference;
        float hue = 0.0f;

        if(color.r == max && color.g == min)
        {
            hue = 5.0f + b;
        }
        else if(color.r == max && color.g != min)
        {
            hue = 1.0f - g;
        }
        else if(color.g == max && color.b == min)
        {
            hue = r + 1.0f;
        }
        else if(color.g == max && color.b != min)
        {
            hue = 3.0f - b;
        }
        else if(color.r == max)
        {
            hue = 3.0f + g;
        }
        else
        {
            hue = 5.0f - r;
        }

        return (int)(hue * 60.0f);
    }

    /// <summary>
    /// Converts hue to rgb
    /// Note, discards value and saturation
    /// Note, hue should be 0->360
    /// http://www.poynton.com/PDFs/coloureq.pdf
    /// </summary>
    static public Color HueToRGB(int hue)
    {
        if(hue > 360 || hue < 0)
        {
            return new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // This is a hack as the colours we want for the player 
        // Cannot be close to black or they will not read well
        // Any hue between 220-270 we decrease the saturation
        float saturation = hue > 220 && hue < 270 ? 0.65f : 0.85f;
        float value = 1.0f;

        Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        float hex = (float)hue / 60.0f;
        float primaryColor = Mathf.Floor(hex);
        float secondaryColour = hex - primaryColor;
        float a = (1.0f - saturation) * value;
        float b = (1.0f - (saturation * secondaryColour)) * value;
        float c = (1.0f - (saturation * (1.0f - secondaryColour))) * value;

        switch((int)primaryColor)
        {
        case 0:
            color.r = value;
            color.g = c;
            color.b = a;
            break;
        case 1:
            color.r = b;
            color.g = value;
            color.b = a;
            break;
        case 2:
            color.r = a;
            color.g = value;
            color.b = c;
            break;
        case 3:
            color.r = a;
            color.g = b;
            color.b = value;
            break;
        case 4:
            color.r = c;
            color.g = a;
            color.b = value;
            break;
        case 5:
            color.r = value;
            color.g = a;
            color.b = b;
            break;
        default:
            color.r = value;
            color.g = c;
            color.b = a;
            break;
        }

        return color;
    }
}