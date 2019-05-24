// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShader : MonoBehaviour
{

    void Start()
    {
        AddContours(transform);

    }

    void AddContours(Transform obj)
    {
        int count = obj.childCount;
        if (count > 0)
        {
            for (int i = 0; i<count; i++)
            {
                AddContours(obj.GetChild(i));
            }
        }

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;
        int range = renderer.materials.Length;

        for (int i = 0; i < range; i++)
        {
            renderer.materials[i].shader = Shader.Find("Outlined/Silhouetted Diffuse");
            renderer.materials[i].SetColor("_OutlineColor", Color.green);
        }
    }
}
