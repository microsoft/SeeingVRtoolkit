// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

public class MoveToCenter : MonoBehaviour {

    public GameObject obj;
	void Start () {

        Renderer renderer = obj.GetComponent<Renderer>();
        transform.position = renderer.bounds.center;
    }

}
