// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Function RTImage is reused from an example on Unity Documentation
// https://docs.unity3d.com/ScriptReference/Camera.Render.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraColorText : MonoBehaviour {

    public Camera cam;
    private Color originColor;
    private FontStyle originStyle;
    private Color targetColor;
    private int originSize;
    private bool adjusted = false;
    Text text;
    Canvas canvas;
    public RenderMode mode;
    private GameObject hint;
    bool whetherPanel = false;
    Image img;
    VerticalWrapMode overflowMode;


    void Start()
    {
        text = transform.parent.GetComponent<Text>();
        originColor = text.color;
        originStyle = text.fontStyle;
        originSize = text.fontSize;
        overflowMode = text.verticalOverflow;

        img = text.transform.parent.GetComponent<Image>();
        if (img != null && img.enabled)
        {
            whetherPanel = true;
        }
        else
        {
            whetherPanel = false;
        }
    }

    void Update()
    {

        if (mode == RenderMode.WorldSpace)
        {
      
            if (InCameraView(transform.gameObject, false) && !adjusted)
            {
            
                if (whetherPanel)
                {

                    targetColor = ContrastColor_Luminance(img.color);
                }
                else
                {
                    targetColor = ContrastColor_Luminance(averageColor(RTImage(cam)));
                    Image curImg = text.transform.parent.GetComponent<Image>();
                    if (curImg == null)
                    {
                        curImg = text.transform.parent.gameObject.AddComponent<Image>();
                        curImg.color = ContrastColor_Luminance(targetColor);
                    }
                    else
                    {
                        curImg.enabled = true;
                    }
                }

         

                text.color = targetColor;
                text.fontStyle = FontStyle.Bold;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                text.fontSize = (originSize + 3);
                adjusted = true;
            }
            else if (!InCameraView(transform.gameObject, false) && adjusted)
            {
                text.color = originColor;
                text.fontSize = originSize;
                text.fontStyle = originStyle;
                text.verticalOverflow = overflowMode;
                if (!whetherPanel)
                {
                    Image curImg = text.transform.parent.GetComponent<Image>();
                    curImg.enabled = false;
                }
                
                adjusted = false;
            }

        }
        else
        {
            Color bgColor = averageColor(RTImage(cam));
            Debug.Log(bgColor.ToString());
            text.color = ContrastColor_Luminance(bgColor);

            text.fontStyle = FontStyle.Bold;
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

    bool InCameraView(GameObject text, bool whetherPrint)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(text.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0.25 && screenPoint.x < 0.75 && screenPoint.y > 0.4 && screenPoint.y < 0.6;
    }


}
