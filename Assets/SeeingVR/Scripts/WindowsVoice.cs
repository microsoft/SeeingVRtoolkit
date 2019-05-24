// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// modified from an open source text to speech on windows for unity
// https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/

using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System;

public class WindowsVoice : MonoBehaviour
{
    [DllImport("WindowsVoice")]
    public static extern void initSpeech();
    [DllImport("WindowsVoice")]
    public static extern void destroySpeech();
    [DllImport("WindowsVoice")]
    public static extern void addToSpeechQueue(string s);
    [DllImport("WindowsVoice")]
    public static extern void clearSpeechQueue();
    [DllImport("WindowsVoice")]
    public static extern void speak(string s);
    [DllImport("WindowsVoice")]
    public static extern void skip();
    [DllImport("WindowsVoice")]
    public static extern void statusMessage(StringBuilder str, int length);
    public static WindowsVoice theVoice = null;

    private string message = "";
    private void Awake()
    {
    }

    void OnEnable()
    {
        if (theVoice == null)
        {
            theVoice = this;
            Log("Initializing speech");
            initSpeech();
            Log("Initializing speech done");
        }
    }
    public void speakVoice(string msg)
    {
        message = msg;
        skip();

        PlayBeep(msg, 1000, 300, 30000, beepPlayed);
    }
    void OnDestroy()
    {
        skip();
        if (theVoice == this)
        {
            Log("Destroying speech");
            destroySpeech();
            theVoice = null;
        }
    }
    public static string GetStatusMessage()
    {
        StringBuilder sb = new StringBuilder(40);
        statusMessage(sb, 40);
        return sb.ToString();
    }

    static private void Log(string s)
    {
        Debug.Log(s);
    }

    public void PlayBeep(string message, UInt16 frequency, int msDuration, UInt16 volume = 16383, EventHandler doneCallback = null)
    {
        var mStrm = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(mStrm);

        const double TAU = 2 * Math.PI;
        int formatChunkSize = 16;
        int headerSize = 8;
        short formatType = 1;
        short tracks = 1;
        int samplesPerSecond = 44100;
        short bitsPerSample = 16;
        short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
        int bytesPerSecond = samplesPerSecond * frameSize;
        int waveSize = 4;
        int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
        int dataChunkSize = samples * frameSize;
        int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
        writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
        writer.Write(fileSize);
        writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
        writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
        writer.Write(formatChunkSize);
        writer.Write(formatType);
        writer.Write(tracks);
        writer.Write(samplesPerSecond);
        writer.Write(bytesPerSecond);
        writer.Write(frameSize);
        writer.Write(bitsPerSample);
        writer.Write(0x61746164); // = encoding.GetBytes("data")
        writer.Write(dataChunkSize);
        {
            double theta = frequency * TAU / (double)samplesPerSecond;
            // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
            // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
            double amp = volume >> 2; // so we simply set amp = volume / 2
            for (int step = 0; step < samples; step++)
            {
                short s = (short)(amp * Math.Sin(theta * (double)step));
                writer.Write(s);
            }
        }

        mStrm.Seek(0, SeekOrigin.Begin);


        if (doneCallback != null)
        {
            doneCallback(this, new EventArgs());
        }


        writer.Close();
        mStrm.Close();
    }

    void beepPlayed(object sender, EventArgs e)
    {
        speak(message);
    }
}
