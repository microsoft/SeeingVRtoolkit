// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAugmentationByShader : MonoBehaviour {

    public bool dynamicScanning = false;
    public bool isAugmented = false;
    private bool priorAugmented = false;

    public bool addColliderToText = false;

    public int fontSizeIncrease = 0;

    private int priorSizeIncrease = 0;

    public bool bold = false;

    public bool priorBold = false;

    void Start()
    {
        Text[] allText = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text text in allText)
        {
            AdjustTextbyShader textShader = text.gameObject.GetComponent<AdjustTextbyShader>();
            if (textShader == null)
            {
                textShader = text.gameObject.AddComponent<AdjustTextbyShader>();
                textShader.fontSizeIncreasement = fontSizeIncrease;
                textShader.bold = bold;
            }

            if (addColliderToText)
            {
                AddFitCollider fitCollider = text.gameObject.GetComponent<AddFitCollider>();
                if (fitCollider == null)
                {
                    fitCollider = text.gameObject.AddComponent<AddFitCollider>();
                }
            }

        }
    }

    void Update () {
        Text[] allText = Resources.FindObjectsOfTypeAll<Text>();

        if (isAugmented && !priorAugmented)
        {

            foreach (Text text in allText)
            {
                AdjustTextbyShader textShader = text.gameObject.GetComponent<AdjustTextbyShader>();
                if (textShader == null)
                {
                    textShader = text.gameObject.AddComponent<AdjustTextbyShader>();
                }
                textShader.fontSizeIncreasement = fontSizeIncrease;
                textShader.bold = bold;
                textShader.isAugmented = true;
            }

            priorAugmented = isAugmented;
            priorSizeIncrease = fontSizeIncrease;
            priorBold = bold;
        }
        else if (!isAugmented && priorAugmented)
        {
            foreach (Text text in allText)
            {
                AdjustTextbyShader textShader = text.gameObject.GetComponent<AdjustTextbyShader>();
                if (textShader != null)
                {
                    textShader.isAugmented = false;
                }
                textShader.fontSizeIncreasement = fontSizeIncrease;
                textShader.bold = bold;
            }

            priorAugmented = isAugmented;
            priorSizeIncrease = fontSizeIncrease;
            priorBold = bold;
        }
        else if (priorSizeIncrease != fontSizeIncrease || priorBold != bold)
        {
            foreach (Text text in allText)
            {
                AdjustTextbyShader textShader = text.gameObject.GetComponent<AdjustTextbyShader>();
                if (textShader == null)
                {
                    textShader = text.gameObject.AddComponent<AdjustTextbyShader>();
                }
                textShader.fontSizeIncreasement = fontSizeIncrease;
                textShader.isAugmented = isAugmented;
                textShader.bold = bold;
            }

            priorAugmented = isAugmented;
            priorSizeIncrease = fontSizeIncrease;
            priorBold = bold;
        }
        else if (isAugmented)
        {

            if (!dynamicScanning) return;
            foreach (Text text in allText)
            {
                AdjustTextbyShader textShader = text.gameObject.GetComponent<AdjustTextbyShader>();
                if (textShader == null)
                {
                    textShader = text.gameObject.AddComponent<AdjustTextbyShader>();
                }
                else
                {
                    textShader.enabled = true;
                }
                textShader.fontSizeIncreasement = fontSizeIncrease;
                textShader.bold = bold;
                textShader.isAugmented = isAugmented;

                if (addColliderToText)
                {
                    AddFitCollider fitCollider = text.gameObject.GetComponent<AddFitCollider>();
                    if (fitCollider == null)
                    {
                        fitCollider = text.gameObject.AddComponent<AddFitCollider>();
                    }
                }

            }
        }


    }
}
