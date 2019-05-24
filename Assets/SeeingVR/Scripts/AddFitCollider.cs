// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// The DrawBounds function is modified based on a post from Unity Community:
// https://answers.unity.com/questions/461588/drawing-a-bounding-box-similar-to-box-collider.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This code is a sample implementation of the work described in
//SeeingVR: A Set of Tools to Make Virtual Reality More Accessible to People with Low Vision
//Yuhang Zhao, Ed Cutrell, Christian Holz, Meredith Ringel Morris, Eyal Ofek, Andy Wilson
//CHI 2019 | May 2019
//
//https://www.microsoft.com/en-us/research/publication/seeingvr-a-set-of-tools-to-make-virtual-reality-more-accessible-to-people-with-low-vision-2/


public class AddFitCollider : MonoBehaviour {
    BoxCollider boxCollider;
    RectTransform rectTransform;

    void Start () {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        Text text = gameObject.GetComponent<Text>();
        if (text != null && boxCollider != null)
        {
            boxCollider.size = new Vector3(text.preferredWidth, text.preferredHeight, 0);

        }
    }


	void Update () {
        Text text = gameObject.GetComponent<Text>();
	    if (boxCollider != null && text != null)
	    {
	        boxCollider.size = new Vector3(text.preferredWidth, text.preferredHeight, 0);
	        DrawBounds(boxCollider.bounds);
        }
	    else if (boxCollider == null)
	    {
	        boxCollider = gameObject.AddComponent<BoxCollider>();
	    }
    }

    void DrawBounds(Bounds bounds)
    {
        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        Vector3 v3FrontTopLeft;
        Vector3 v3FrontTopRight;
        Vector3 v3FrontBottomLeft;
        Vector3 v3FrontBottomRight;
        Vector3 v3BackTopLeft;
        Vector3 v3BackTopRight;
        Vector3 v3BackBottomLeft;
        Vector3 v3BackBottomRight;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner


        Color color = Color.green;

        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
    }
}
