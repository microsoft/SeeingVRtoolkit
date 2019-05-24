// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;
using Valve.VR;

public struct DepthPointerEventArgs
{
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void DepthPointerEventHandler(object sender, DepthPointerEventArgs e);

public class DepthLaser : MonoBehaviour {

    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    public bool addRigidBody = false;
    public Transform reference;
    public event DepthPointerEventHandler PointerIn;
    public event DepthPointerEventHandler PointerOut;
    public event DepthPointerEventHandler PointerStay;
    public float rotateAngle = 0;
    private float priorRotate = 0;
    public float shiftDistance = 0;

    Transform previousContact = null;
    public GameObject hint;
    private int count = 0;
    public bool isDepthMeasured = false;
    private bool priorMeasured = false;

    void Start()
    {
        holder = new GameObject("_depth_laser_holder");
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;
        hint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hint.name = "_depth_laser_hint";
        hint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        hint.GetComponent<SphereCollider>().enabled = false;
        hint.GetComponent<Renderer>().material.color = Color.green;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.name = "_depth_laser_pointer";
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;

        holder.transform.RotateAround(transform.position, transform.right, rotateAngle);
        holder.transform.localPosition = new Vector3(0, shiftDistance, 0);
        priorRotate = rotateAngle;

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

        hint.GetComponent<MeshRenderer>().material = newMaterial;

        holder.SetActive(true);
        pointer.SetActive(false);
        hint.SetActive(false);
    }

    public virtual void OnPointerIn(DepthPointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(DepthPointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }

    public virtual void OnPointStay(DepthPointerEventArgs e)
    {
        if (PointerStay != null)
            PointerStay(this, e);
    }

    void Update()
    {

        holder.transform.RotateAround(transform.position, transform.right, rotateAngle - priorRotate);
        priorRotate = rotateAngle;
        holder.transform.localPosition = new Vector3(0, shiftDistance, 0);

        if (isDepthMeasured && !priorMeasured)
        {
            holder.SetActive(true);
            pointer.SetActive(true);
            hint.SetActive(true);
            priorMeasured = isDepthMeasured;
            return;
        }
        else if (!isDepthMeasured && priorMeasured)
        {
            holder.SetActive(false);
            pointer.SetActive(false);
            hint.SetActive(false);
            priorMeasured = isDepthMeasured;
        }
        else if (!isDepthMeasured)
        {
            return;

        }

        float dist = 100f;

        Ray raycast = new Ray(holder.transform.position, pointer.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit, Mathf.Infinity);

        hint.transform.position = hit.point;

        if (previousContact && previousContact == hit.transform)
        {
            count++;
            if (count == 20)
            {
                DepthPointerEventArgs args = new DepthPointerEventArgs();
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
            DepthPointerEventArgs args = new DepthPointerEventArgs();
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            DepthPointerEventArgs argsIn = new DepthPointerEventArgs();
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
        if (bHit)
        {
            dist = hit.distance;
        }



        bool actionWasActivated = SteamVR_Actions.seeingVR_default_MakeLaserThicker.stateDown;

        if (actionWasActivated)
        {
            pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
        }
        else if (SteamVR_Actions.seeingVR_default_MakeLaserThicker.stateUp)
        {
            pointer.transform.localScale = new Vector3(thickness, thickness, dist);
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);

        pointer.GetComponent<Renderer>().material.color = color;
        hint.GetComponent<Renderer>().material.color = color;
    }

}
