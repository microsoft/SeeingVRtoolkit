// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This code is a sample implementation of the work described in
//SeeingVR: A Set of Tools to Make Virtual Reality More Accessible to People with Low Vision
//Yuhang Zhao, Ed Cutrell, Christian Holz, Meredith Ringel Morris, Eyal Ofek, Andy Wilson
//CHI 2019 | May 2019
//
//https://www.microsoft.com/en-us/research/publication/seeingvr-a-set-of-tools-to-make-virtual-reality-more-accessible-to-people-with-low-vision-2/


public class AdjustMagnificationLevel : MonoBehaviour {

    public float magnificationLevel = 0;
    public Camera c;
    private float FOV = 60;
    void Start () {

	}

	void Update () {
		float angle = Mathf.Atan(Mathf.Tan((FOV/2.0f) * Mathf.PI/180)/magnificationLevel) * 180 * 2/Mathf.PI;
		c.fieldOfView = angle;
    }
}
