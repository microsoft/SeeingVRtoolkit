// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(VoiceLaser))]
public class VoiceComponent : MonoBehaviour {

    private VoiceLaser laserPointer;

    string priorContent = "";
    string currentContent = "";

    public bool TextToSpeech = false;
    public bool objectDescription = true;

    private string priorObj = "";
    private string currentObj = "";
    private GameObject priorGameObject;

    private void OnEnable()
    {
        laserPointer = GetComponent<VoiceLaser>();
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut += HandlePointerOut;
        laserPointer.PointerStay += HandlePointerStay;
    }

    private void HandlePointerIn (object sender, TTSPointerEventArgs e)
    {
    }

    private void HandlePointerStay(object sender, TTSPointerEventArgs e)
    {
        if (objectDescription)
        {
            currentObj = getDescription(e.target.gameObject.transform);
            if (currentObj != null && e.target.gameObject != priorGameObject)
            {
                Debug.Log("described: " + currentObj);
                var thread = new Thread(() => { WindowsVoice.theVoice.speakVoice(currentObj); });
                thread.Start();
                priorObj = currentObj;
                priorGameObject = e.target.gameObject;
            }

        }

        if (TextToSpeech)
        {
            Text text = e.target.gameObject.GetComponent<Text>();

            if (text != null)
            {
                currentContent = text.text;
                if (priorContent != currentContent)
                {
                    Debug.Log("text: " + currentContent);
                    var thread = new Thread(() => { WindowsVoice.theVoice.speakVoice(currentContent); });
                    thread.Start();

                    print(text.text + " " + text.gameObject.name + " " + text.gameObject.transform.parent.name);
                    priorContent = currentContent;
                }

                return;
            }

            text = e.target.gameObject.GetComponentInChildren<Text>();
            if (text != null)
            {

                currentContent = text.text;
                if (priorContent != currentContent)
                {
                    Debug.Log("text: " + currentContent);
                    var thread = new Thread(() => { WindowsVoice.theVoice.speakVoice(currentContent); });
                    thread.Start();

                    priorContent = currentContent;
                }

                return;
            }

            if (text == null)
            {
                priorContent = "";
            }
        }

    }

    string getDescription(Transform obj)
    {
        Transform curObject = obj;
        string description = curObject.gameObject.getDescription();
        while (description == null || description.Equals(""))
        {
            curObject = curObject.parent;
            if (curObject != null)
            {
                description = curObject.gameObject.getDescription();
            }
            else
            {
                return null;
            }
        }

        return description;
    }

    private void HandlePointerOut(object sender, TTSPointerEventArgs e)
    {

    }

}
