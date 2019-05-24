// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HighlightSalience : MonoBehaviour {

    public bool highlight = false;
    private bool priorHighlight = false;

    public bool Guideline = false;
    private bool priorGuidline = false;


    public bool dynamicScanning = false;
    public Color highlightColor = Color.green;
    private Color priorHighlighColor = Color.white;

    public Color guidelineColor = Color.red;
    private Color priorGuidlineColor = Color.white;

	public float forwardFactor = 0.5f;
	private float priorForwardFactor = 0.5f;


	public float radius = 0.25f;
	private float priorRadius = 0.25f;

    void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy && obj.isSalience())
            {
               AddContours contours = obj.AddComponent<AddContours>();
            }
        }
    }

	void Update () {
	    if (highlight != priorHighlight || priorGuidline != Guideline || highlightColor != priorHighlighColor
	        || guidelineColor != priorGuidlineColor || priorForwardFactor != forwardFactor || priorRadius != radius)
	    {
	        GameObject[] allObjects = FindObjectsOfType<GameObject>();
	        foreach (GameObject obj in allObjects)
	        {
	            if (obj.activeInHierarchy && obj.isSalience())
	            {
	                AddContours contours = obj.GetComponent<AddContours>();
	                if (contours == null)
	                {
	                    contours = obj.AddComponent<AddContours>();

	                }

	                contours.whetherHighlighted = highlight;
	                contours.whetherLink = Guideline;
	                contours.color = highlightColor;
	                contours.guidlineColor = guidelineColor;
		            contours.forwardFactor = forwardFactor;
		            contours.radius = radius;

	            }
	        }

	        priorHighlight = highlight;
	        priorGuidline = Guideline;
	        priorHighlighColor = highlightColor;
	        priorGuidlineColor = guidelineColor;
		    priorForwardFactor = forwardFactor;
		    priorRadius = radius;
	    }

        if (dynamicScanning)
	    {
	        GameObject[] allObjects = FindObjectsOfType<GameObject>();
	        foreach (GameObject obj in allObjects)
	        {
	            if (obj.activeInHierarchy && obj.isSalience())
	            {
	                AddContours contours = obj.GetComponent<AddContours>();
	                if (contours == null)
	                {
	                    contours = obj.AddComponent<AddContours>();

	                }

	                contours.whetherHighlighted = highlight;
	                contours.whetherLink = Guideline;
	                contours.color = highlightColor;
	                contours.guidlineColor = guidelineColor;
		            contours.forwardFactor = forwardFactor;
		            contours.radius = radius;

                }
            }

		    priorHighlight = highlight;
		    priorGuidline = Guideline;
		    priorHighlighColor = highlightColor;
		    priorGuidlineColor = guidelineColor;
		    priorForwardFactor = forwardFactor;
		    priorRadius = radius;

        }

    }
}
