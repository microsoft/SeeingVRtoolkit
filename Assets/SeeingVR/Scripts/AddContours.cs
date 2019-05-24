// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// The CalculateCubicBezierPoint function is reused from a blog on Gamasutra:
// https://www.gamasutra.com/blogs/VivekTank/20180806/323709/How_to_work_with_Bezier_Curve_in_Games_with_Unity.php

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

//This code is a sample implementation of the work described in
//SeeingVR: A Set of Tools to Make Virtual Reality More Accessible to People with Low Vision
//Yuhang Zhao, Ed Cutrell, Christian Holz, Meredith Ringel Morris, Eyal Ofek, Andy Wilson
//CHI 2019 | May 2019
//
//https://www.microsoft.com/en-us/research/publication/seeingvr-a-set-of-tools-to-make-virtual-reality-more-accessible-to-people-with-low-vision-2/



public class AddContours : MonoBehaviour {

    public bool whetherHighlighted = false;
    private bool priorHilighted = false;
    public Color color = Color.green;
    public Color guidlineColor  = Color.red;
    private bool priorSalience = false;

    public  float forwardFactor = 0.5f;
    public float radius = 0.25f;

    Renderer renderer_comp;
    Camera main_camera;
    private Dictionary<int, Material[]> shaderMap = new Dictionary<int, Material[]>();
    private Dictionary<int, Material[]> recolorMap = new Dictionary<int, Material[]>();
    private LineRenderer line;
    private GameObject cylindar;
    private GameObject lineContainer;

    public bool whetherLink = false;
    AdjustMagnificationLevel mag;

    void Start () {
        renderer_comp = GetComponent<Renderer>();
        main_camera = Camera.main;
        lineContainer = new GameObject("_line_container");
        line = lineContainer.AddComponent<LineRenderer>();
        lineContainer.transform.parent = transform;
        lineContainer.name = "Guidline";
        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        mag = Camera.main.GetComponentInChildren<AdjustMagnificationLevel>(true);
        priorSalience = gameObject.isSalience();
    }

    void Update () {

        if (!gameObject.isSalience() && priorSalience)
        {

            if (whetherLink)
                lineContainer.SetActive(false);
            if (whetherHighlighted)
            {
                RemoveHighlights(transform);
            }

            priorSalience = gameObject.isSalience();
            priorHilighted = false;

            return;
        }
        else if (gameObject.isSalience())
        {

            if (whetherHighlighted && !priorHilighted)
            {

                AddHighlights(transform);
                priorHilighted = whetherHighlighted;
            }
            else if (!whetherHighlighted && priorHilighted)
            {
                RemoveHighlights(transform);
                priorHilighted = whetherHighlighted;
            }

            if (whetherHighlighted)
            {
                UpdateShaderParameters(transform);
            }

            if (whetherLink)
            {
                AddLinks();
            }
            else
            {
                lineContainer.SetActive(false);
            }
        }

        priorSalience = gameObject.isSalience();

    }

    void AddLinks()
    {
        Vector3 objCenter = GetBounds(transform).center;
        lineContainer.SetActive(true);

        Camera cam = Camera.main;
        ;

        Vector3 cameraCenter = cam.transform.position + cam.transform.forward * forwardFactor;
        Vector3 p2 = Vector3.Normalize(objCenter - cameraCenter) * radius + cameraCenter;

        if (AtBackofCamera(objCenter))
        {
            Vector3 lineCenter = p2 + Vector3.Normalize(objCenter - p2) * 0.2f;
            Vector3 normal = Vector3.Cross(p2 - cam.transform.position, objCenter - cam.transform.position);
            Vector3 direction = cam.transform.right;

            Vector3 screenPoint = main_camera.WorldToViewportPoint(objCenter);
            float factor = 0.5f;
            if (screenPoint.x >= 0.5)
            {
                factor = factor * (0.9f - screenPoint.x) / 0.4f;
            }
            else
            {
                factor = factor * (screenPoint.x - 0.1f) / 0.4f;
            }

            Vector3 middle = lineCenter - direction * factor;
            if (isLeft(reMoveY(objCenter), reMoveY(p2), reMoveY(cam.transform.position)))
                middle = lineCenter + direction * factor;
            Debug.DrawLine(p2, objCenter, Color.blue);
            DrawCurve(objCenter, middle, p2);
            line.startColor = guidlineColor;
            line.endColor = guidlineColor;
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;
        }
        else
        {
            Vector3[] points = new Vector3[2];
            points[0] = objCenter;
            points[1] = p2;
            line.positionCount = 2;
            line.SetPositions(points);
            line.startColor = guidlineColor;
            line.endColor = guidlineColor;
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;
        }

    }

    Vector2 reMoveY(Vector3 original)
    {
        return new Vector2(original.x, original.z);
    }

    bool isLeft(Vector2 a, Vector2 b, Vector2 point)
    {
        return ((b.x - a.x)*(point.y - a.y) - (b.y - a.y)*(point.x - a.x)) > 0;
    }


    void DrawCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {

        int SEGMENT_COUNT = 50;
        Vector3 lineCenter = (p2 + p0) / 2;
        Vector3[] points = new Vector3[SEGMENT_COUNT+1];
        points[0] = p0;
        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float) SEGMENT_COUNT;
            Vector3 pixel = CalculateCubicBezierPoint(t, p0, p1, p2);
            points[i] = pixel;
            Debug.DrawLine(lineCenter, pixel, Color.yellow);
        }

        line.positionCount = SEGMENT_COUNT + 1;
        line.SetPositions(points);
    }

    float pointToLineDistance(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        return  Vector3.Magnitude(Vector3.Cross(point - linePoint1, point - linePoint2))/ Vector3.Distance(linePoint2, linePoint1);
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;


        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    void AddHighlights(Transform obj)
    {

        int count = obj.childCount;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (!obj.GetChild(i).name.Equals("Guidline"))
                    AddHighlights(obj.GetChild(i));
            }
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;
        Vector4 vec = new Vector4(0, 0, 0, 1);
        Vector3 tmp = renderer.bounds.center - obj.transform.position;
        vec.x = tmp.x;
        vec.y = tmp.y;
        vec.z = tmp.z;


        if (renderer.materials != null && renderer.materials.Length != 0)
        {
            if (!shaderMap.ContainsKey(obj.GetInstanceID()))
            {
                shaderMap.Add(obj.GetInstanceID(), (Material[])renderer.materials);
            }
            else
            {
                shaderMap[obj.GetInstanceID()] = (Material[])renderer.materials;
            }
        }



        int range = renderer.materials.Length;
        Material[] original = renderer.materials;

        renderer.materials = new Material[range];

        for (int i = 0; i < range; i++)
        {
            renderer.materials[i].CopyPropertiesFromMaterial(original[i]);
            renderer.materials[i].shader = Shader.Find("Custom/Outline");

            renderer.materials[i].SetColor("_OutlineColor", color);
            renderer.materials[i].SetVector("_CenterToPivot", vec);
            renderer.materials[i].SetFloat("_OutlineWidth", 1.1f);
        }
    }

    void UpdateShaderParameters(Transform obj)
    {
        int count = obj.childCount;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                UpdateShaderParameters(obj.GetChild(i));
            }
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;
        Vector4 vec = new Vector4(0, 0, 0, 1);
        Vector3 tmp = renderer.bounds.center - obj.position;
        vec.x = tmp.x;
        vec.y = tmp.y;
        vec.z = tmp.z;

        int range = renderer.materials.Length;

        for (int i = 0; i < range; i++)
        {
            renderer.materials[i].SetColor("_OutlineColor",color);
            renderer.materials[i].SetVector("_CenterToPivot", vec);
        }
    }

    void RemoveHighlights(Transform obj)
    {
        int count = obj.childCount;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                RemoveHighlights(obj.GetChild(i));
            }
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        if (shaderMap.ContainsKey(obj.GetInstanceID()))
        {
            renderer.materials = shaderMap[obj.GetInstanceID()];
        }


    }

    bool InCameraView(Vector3 point)
    {

        Vector3 screenPoint = main_camera.WorldToViewportPoint(point);
        if (mag == null || !mag.gameObject.activeSelf)
            return screenPoint.z > 0 && screenPoint.x > 0.4 && screenPoint.x < 0.6 && screenPoint.y > 0.4 && screenPoint.y < 0.6;
        else
            return screenPoint.z > 0 && screenPoint.x > 0.45 && screenPoint.x < 0.55 && screenPoint.y > 0.45 && screenPoint.y < 0.55;

    }

    bool AtBackofCamera(Vector3 point)
    {
        Vector3 screenPoint = main_camera.WorldToViewportPoint(point);
        return  screenPoint.z < 0 && screenPoint.x > 0.1 && screenPoint.x < 0.9;
    }

    Bounds GetBounds(Transform obj)
    {
        Bounds bounds;
        Renderer parentRender = obj.GetComponent<Renderer>();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (parentRender != null)
            bounds = parentRender.bounds;
        else
        {
            Vector3 center = Vector3.zero;
            int count = 0;
            foreach (Renderer render in renderers)
            {
                if (!render.gameObject.name.Equals("Guidline"))
                {
                    center += render.bounds.center;
                    count++;
                }

            }
            center /= count; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
            bounds = new Bounds(center, Vector3.zero);
        }


        foreach (Renderer render in renderers)
        {
            if (!render.gameObject.name.Equals("Guidline"))
                bounds.Encapsulate(render.bounds);
        }

        return bounds;
    }
}
