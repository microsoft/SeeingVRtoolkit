// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;
using Valve.VR;

public struct TTSPointerEventArgs
{
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void TTSPointerEventHandler(object sender, TTSPointerEventArgs e);


public class VoiceLaser : MonoBehaviour {

    public bool active = true;
    public Color color = Color.green;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event TTSPointerEventHandler PointerIn;
    public event TTSPointerEventHandler PointerOut;
    public event TTSPointerEventHandler PointerStay;
    public float rotateAngle = 0;
    private float priorRotate = 0;
    public float shiftDistance = 0;

    Transform previousContact = null;
    public int count = 0;
    private VoiceComponent voiceComponent;

    void Start()
    {
        holder = new GameObject("_voice_laser_holder");
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;
      
        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;

        holder.transform.RotateAround(transform.position, transform.right, rotateAngle);
        priorRotate = rotateAngle;
        holder.transform.localPosition = new Vector3(0, shiftDistance, 0);

        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

        voiceComponent = GetComponent<VoiceComponent>();
    }

    public virtual void OnPointerIn(TTSPointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(TTSPointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }

    public virtual void OnPointStay(TTSPointerEventArgs e)
    {
        if (PointerStay != null)
            PointerStay(this, e);
    }

    void Update()
    {
        holder.transform.RotateAround(transform.position, transform.right, rotateAngle - priorRotate);
        priorRotate = rotateAngle;
        holder.transform.localPosition = new Vector3(0, shiftDistance, 0);

        if (!voiceComponent.TextToSpeech && !voiceComponent.objectDescription)
        {
            pointer.SetActive(false);
            return;
        }
        else
        {
            pointer.SetActive(true);
        }

        if (!isActive)
        {
            isActive = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        float dist = 100f;

        Ray raycast = new Ray(holder.transform.position, pointer.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact == hit.transform)
        {
            count++;
            if (count == 20)
            {
                TTSPointerEventArgs args = new TTSPointerEventArgs();
                args.distance = hit.distance;
                args.flags = 0;
                args.target = previousContact;
                OnPointStay(args);
            }
        }
        else
        {
            count = 0;
        }

        if (previousContact && previousContact != hit.transform)
        {
            TTSPointerEventArgs args = new TTSPointerEventArgs();
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            TTSPointerEventArgs argsIn = new TTSPointerEventArgs();
            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }
        if (!bHit)
        {
            previousContact = null;
        }
        if (bHit && hit.distance < 100f)
        {
            dist = hit.distance;
        }

        if (SteamVR_Actions.seeingVR_default_MakeLaserThicker.stateDown)
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
        }
        else
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);


    }
}
