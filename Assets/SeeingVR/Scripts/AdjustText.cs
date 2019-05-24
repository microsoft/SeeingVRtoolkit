// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Function RTImage is reused from an example from Unity Documentation:
// https://docs.unity3d.com/ScriptReference/Camera.Render.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustText : MonoBehaviour {

    private Camera cam;
    private Color originColor;
    private FontStyle originStyle;
    private Font originFont;
    private bool adjusted = false;

    void Start()
    {
        Text text = transform.GetComponent<Text>();
        originColor = text.color;
        originStyle = text.fontStyle;
        originFont = text.font;
        cam = gameObject.AddComponent<Camera>();
    }

    void Update()
    {
        if (InCameraView(transform.gameObject) && !adjusted)
        {
            Color bgColor = averageColor(RTImage(cam));
            Debug.Log(bgColor.ToString());
            Text text = transform.GetComponent<Text>();
            text.color = ContrastColor_Luminance(bgColor);

            text.fontStyle = FontStyle.Bold;
            adjusted = true;
        }
        else if (!InCameraView(transform.gameObject) && adjusted)
        {
            Text text = transform.GetComponent<Text>();
            text.color = originColor;

            text.fontStyle = originStyle;
            text.font = originFont;
            adjusted = false;
        }

    }

    Texture2D RTImage(Camera cam)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        return image;
    }

    Color averageColor(Texture2D tex)
    {
        float red = 0;
        float green = 0;
        float blue = 0;

        Color pixel;
        int count = 0;

        int textWidth = tex.width;
        int textHeight = tex.height;

        for (int x = 0; x < textWidth; x++)
        {
            for (int y = 0; y < textHeight; y++)
            {
                pixel = tex.GetPixel(x, y);

                red += pixel.r;
                green += pixel.g;
                blue += pixel.b;

                count++;
            }
        }

        red /= count;
        green /= count;
        blue /= count;

        Color average = new Color(red, green, blue);
        return average;
    }

    Color ContrastColor_Luminance(Color color)
    {
        int d = 0;

        double a = 1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b);
        if (a <= 0.6)
        {
            d = 0;
        }
        else
            d = 1;

        return new Color(d, d, d);
    }

    Color ContrastColor_HSV(Color color)
    {
        float H, S, V;
        Color.RGBToHSV(color, out H, out S, out V);
        H = 1 - H;
        return Color.HSVToRGB(H, S, V);
    }

    bool InCameraView(GameObject text)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(text.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0.25 && screenPoint.x < 0.75 && screenPoint.y > 0.4 && screenPoint.y < 0.6;
    }
}
