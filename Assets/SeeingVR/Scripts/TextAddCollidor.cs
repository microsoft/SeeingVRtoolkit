// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAddCollidor : MonoBehaviour {

	void Update () {
        Text[] allObjects = FindObjectsOfType<Text>();
        foreach(var text in allObjects)
        {
            GameObject obj = text.gameObject;
            if (!obj.GetComponent<AddFitCollider>())
            {
                obj.AddComponent<AddFitCollider>();
            }
        }
    }


}
