// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextColor : MonoBehaviour {

	void Start () {

	}

	void Update () {
        Vector3 direction = transform.position - Camera.main.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            GameObject obj_bg = hit.transform.gameObject;
            Color clr = obj_bg.GetComponent<Renderer>().material.color;
            Debug.Log(clr.ToString());

            Texture2D texture = (Texture2D) obj_bg.GetComponent<Renderer>().material.mainTexture;
            Color average = averageColor(texture);

            transform.GetComponent<Text>().color = ContrastColor(average);
        }
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

    Color ContrastColor(Color color)
    {
        int d = 0;

        double a = 1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b);
        if (a < 0.5)
            d = 0;
        else
            d = 1;

        return new Color(d, d, d);
    }
}
