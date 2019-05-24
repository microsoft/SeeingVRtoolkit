// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOutline : MonoBehaviour {

    bool duplicated = false;
	void Start () {
        if(!duplicated)
        {
            GameObject button = GameObject.Instantiate(transform.gameObject);
            button.GetComponent<ButtonOutline>().enabled = false;
            button.transform.parent = transform.parent;
            int index = transform.GetSiblingIndex();
            if (index == 0)
                button.transform.SetSiblingIndex(index);
            else
                button.transform.SetSiblingIndex(index - 1);
            var rect = button.GetComponent<RectTransform>();
            var orRect = transform.GetComponent<RectTransform>();
            rect.rotation = orRect.rotation;
            rect.position = orRect.position;
            rect.pivot = orRect.pivot;
            rect.localScale = 1.3f * orRect.localScale;

            ColorBlock cb = button.GetComponent<Button>().colors;
            cb.normalColor = Color.green;
            button.GetComponent<Button>().colors = cb;

            foreach (Transform child in button.transform)
            {
                print(child);
                Destroy(child.gameObject);
            }

            print("duplicated!");
            duplicated = true;
        }


	}

}
