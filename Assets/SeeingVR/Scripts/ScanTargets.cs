// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTargets : MonoBehaviour {
    Camera camera_comp;
    GameObject prior;
    public GameObject cursor;
	void Start () {
        camera_comp = GetComponent<Camera>();
	}

	void Update () {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f, Physics.DefaultRaycastLayers))
        {
            GameObject target = hitInfo.transform.gameObject;
            cursor.transform.position = hitInfo.point;

            Debug.LogWarning(target);
            if (target != prior)
            {
                if (prior != null)
                {
                    prior.GetComponent<AddContours>().enabled = false;
                }

                if (target.GetComponent<AddContours>() == null)
                {
                    target.AddComponent<AddContours>();
                }
                else
                {
                    target.GetComponent<AddContours>().enabled = true;
                }

                prior = target;
            }

        }
	}
}
