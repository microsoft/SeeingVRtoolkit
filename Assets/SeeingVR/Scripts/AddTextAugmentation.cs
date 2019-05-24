// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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



public class AddTextAugmentation : MonoBehaviour {

    public GameObject textAugmentation;
    public bool dynamicScanning = false;
    public bool isAugmented = true;
    void Start()
    {
        Text[] allText = FindObjectsOfType<Text>();
        foreach (Text text in allText)
        {

            Canvas canvas = text.transform.parent.GetComponent<Canvas>();
            if (canvas != null)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                    if (textAdj == null)
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = text.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                        colorText.mode = canvas.renderMode;
                        colorText.cam = textAug.GetComponent<Camera>();
                        textAug.GetComponent<Camera>().fieldOfView = 120;
                        textAug.transform.localPosition = new Vector3(0, -10, -5);
                    }
                    else
                    {
                        CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                        if (!colorText.enabled)
                        {
                            colorText.enabled = true;
                        }
                    }
                }
                else
                {
                    if (!Camera.main.transform.Find("TextAdjustment(Clone)"))
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = Camera.main.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        textAug.GetComponent<Camera>().fieldOfView = 50;
                    }

                    Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                    if (textAdj == null)
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = text.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                        colorText.mode = canvas.renderMode;
                        colorText.cam = Camera.main.transform.Find("TextAdjustment(Clone)").GetComponent<Camera>();
                    }
                    else
                    {
                        CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                        if (!colorText.enabled)
                        {
                            colorText.enabled = true;
                        }
                    }
                }
            }
            else
            {
                Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                if (textAdj == null)
                {
                    GameObject textAug = Instantiate(textAugmentation);
                    textAug.transform.parent = text.transform;
                    textAug.transform.localPosition = Vector3.zero;
                    textAug.transform.localRotation = Quaternion.identity;
                    CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                    colorText.mode = RenderMode.WorldSpace;
                    colorText.cam = textAug.GetComponent<Camera>();
                    textAug.GetComponent<Camera>().fieldOfView = 120;
                    textAug.transform.localPosition = new Vector3(0, 2, 0);
                }
                else
                {
                    CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                    if (!colorText.enabled)
                    {
                        colorText.enabled = true;
                    }
                }
            }

        }
    }

    void Update()
    {
        if (!dynamicScanning) return;


        Text[] allText = FindObjectsOfType<Text>();
        foreach (Text text in allText)
        {

            Canvas canvas = text.transform.parent.GetComponent<Canvas>();
            if (canvas != null)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                    if (textAdj == null)
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = text.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                        colorText.mode = canvas.renderMode;
                        colorText.cam = textAug.GetComponent<Camera>();
                        textAug.GetComponent<Camera>().fieldOfView = 120;
                        textAug.transform.localPosition = new Vector3(0, -10, -5);
                    }
                    else
                    {
                        CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                        if (!colorText.enabled)
                        {
                            colorText.enabled = true;
                        }
                    }
                }
                else
                {
                    if (!Camera.main.transform.Find("TextAdjustment(Clone)"))
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = Camera.main.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        textAug.GetComponent<Camera>().fieldOfView = 50;
                    }

                    Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                    if (textAdj == null)
                    {
                        GameObject textAug = Instantiate(textAugmentation);
                        textAug.transform.parent = text.transform;
                        textAug.transform.localPosition = Vector3.zero;
                        textAug.transform.localRotation = Quaternion.identity;
                        CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                        colorText.mode = canvas.renderMode;
                        colorText.cam = Camera.main.transform.Find("TextAdjustment(Clone)").GetComponent<Camera>();
                    }
                    else
                    {
                        CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                        if (!colorText.enabled)
                        {
                            colorText.enabled = true;
                        }
                    }
                }
            }
            else
            {
                Transform textAdj = text.transform.Find("TextAdjustment(Clone)");
                if (textAdj == null)
                {
                    GameObject textAug = Instantiate(textAugmentation);
                    textAug.transform.parent = text.transform;
                    textAug.transform.localPosition = Vector3.zero;
                    textAug.transform.localRotation = Quaternion.identity;
                    CameraColorText colorText = textAug.AddComponent<CameraColorText>();
                    colorText.mode = RenderMode.WorldSpace;
                    colorText.cam = textAug.GetComponent<Camera>();
                    textAug.GetComponent<Camera>().fieldOfView = 120;
                    textAug.transform.localPosition = new Vector3(0, 2, 0);
                }
                else
                {
                    CameraColorText colorText = textAdj.GetComponent<CameraColorText>();
                    if (!colorText.enabled)
                    {
                        colorText.enabled = true;
                    }
                }
            }

        }
    }

}
