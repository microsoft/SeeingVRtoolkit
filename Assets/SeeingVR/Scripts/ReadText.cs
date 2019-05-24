// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class ReadText : MonoBehaviour {

    private Socket sendSocket;
    private IPAddress sendAddress;
    private IPEndPoint endPoint;
    private bool whethersend = false;
    private String audioPath;
    GameObject TextToSpeech;
	void Start () {
        sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sendAddress = IPAddress.Parse("127.0.0.1");
        endPoint = new IPEndPoint(sendAddress, 11000);
	}

	void Update () {
        Text text = transform.GetComponent<Text>();

        if (InCameraView(text))
        {

            if (!whethersend)
            {
                Debug.Log("GetInView: " + text.text);
                Debug.Log("read: " + text.text);
                WindowsVoice.theVoice.speakVoice(text.text);

                print("send");
                sendSignal("test");
                String context = text.text;
                whethersend = true;
            }

        }
        else
        {
            if (whethersend)
            {
                Debug.Log("GetOutView: " + text.text);
                whethersend = false;
            }
        }
	}

    bool InCameraView(Text text)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(text.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0.25 && screenPoint.x < 0.75 && screenPoint.y > 0.4 && screenPoint.y < 0.6;
    }

    private void sendSignal(string signal)
    {
        byte[] sendBuffer = System.Text.Encoding.ASCII.GetBytes(signal);
        try
        {
            sendSocket.SendTo(sendBuffer, endPoint);

            byte[] bytes = new byte[1000];
            sendSocket.Receive(bytes);
            String msg = Encoding.UTF8.GetString(bytes);
            print(msg + " " +transform.gameObject.name);
        }
        catch (Exception)
        {
            print("socket problem!");
        }
    }
}
