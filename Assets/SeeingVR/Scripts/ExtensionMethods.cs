// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;

public static class ExtensionMethods {


    public static string getDescription(this GameObject obj)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            return obj.GetComponent<AccessibilityTags>().Description;
        return null;
    }

    public static void setDescription(this GameObject obj, string value)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            tags.Description = value;
    }

    public static bool isSalience(this GameObject obj)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            return tags.isSalience;
        return false;
    }

    public static void setSalience(this GameObject obj, bool value)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            tags.isSalience = value;
    }

    public static bool isWholeObject(this GameObject obj)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            return tags.isWholeObject;
        return false;
    }

    public static void setWholeObject(this GameObject obj, bool value)
    {
        AccessibilityTags tags = obj.GetComponent<AccessibilityTags>();
        if (tags != null)
            tags.isWholeObject = value;
    }
}
