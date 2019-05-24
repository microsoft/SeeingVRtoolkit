// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustTextbyShader : MonoBehaviour {

    private Material orignalMat;
    private Material invertMat;
    private Color originColor;
    private FontStyle originStyle;
    private Font originFont;
    private int originSize;
    VerticalWrapMode originalOFMode;

    public bool isAugmented = false;
    public int fontSizeIncreasement = 0;
	public bool bold = false;

    private bool priorAugmented = false;

    private Text text;

    void Start () {
	    invertMat = new Material(Shader.Find("GrabPassInvert"));
        if (invertMat == null)
        {
            Debug.Log("no material!");
        }
        text = transform.GetComponent<Text>();
        orignalMat = text.material;
        originColor = text.color;
        originStyle = text.fontStyle;
        originFont = text.font;
        originSize = text.fontSize;
        originalOFMode = text.verticalOverflow;
        originFont = text.font;
    }


	void Update () {
	    if (isAugmented && !priorAugmented)
	    {
	        text.material = invertMat;
		    if (bold)
		    {
			    text.fontStyle = FontStyle.Bold;
		    }
	        text.verticalOverflow = VerticalWrapMode.Overflow;
	        text.fontSize = (originSize + fontSizeIncreasement);
	        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
	        priorAugmented = isAugmented;

	    }
        else if (!isAugmented && priorAugmented)
	    {
	        text.material = orignalMat;
	        text.fontSize = originSize;
	        text.fontStyle = originStyle;
	        text.verticalOverflow = originalOFMode;
	        text.font = originFont;
	        priorAugmented = isAugmented;
	    }
	    else
	    {
	        if (isAugmented)
	        {
	            text.fontSize = (originSize + fontSizeIncreasement);
		        if (bold)
		        {
			        text.fontStyle = FontStyle.Bold;
		        }
		        else
		        {
			        text.fontStyle = originStyle;
		        }
            }
	    }
	}
}
