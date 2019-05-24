using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{

    public bool showTools;
    public bool copyPosition;
    public bool copyRotation;
    public bool copyScale;
    public bool pastePosition;
    public bool pasteRotation;
    public bool pasteScale;
    public bool selectionNullError;
    public string description;
    public bool salience;
    public bool wholeObject;

    public override void OnInspectorGUI()
    {

        Transform t = (Transform)target;

        if (t.gameObject.GetComponent<AccessibilityTags>() == null)
        {
            t.gameObject.AddComponent<AccessibilityTags>();
            EditorUtility.SetDirty(t);
        }

        // Replicate the standard transform inspector gui
        EditorGUIUtility.LookLikeControls();
        EditorGUI.indentLevel = 0;
        Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
        EditorGUIUtility.LookLikeInspector();

        EditorGUILayout.Space();
        GUILayout.Label("Accessibility Tags", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Description", GUILayout.Width(75));
        EditorGUIUtility.labelWidth = 100;

        if (t.gameObject.GetComponent<AccessibilityTags>() != null && t.gameObject.GetComponent<AccessibilityTags>().Description != null)
            description = GUILayout.TextField(t.gameObject.GetComponent<AccessibilityTags>().Description);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("isSalient", GUILayout.Width(100));
        salience = EditorGUILayout.Toggle(t.gameObject.isSalience(), GUILayout.Width(75));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("isWholeObject", GUILayout.Width(100));
        wholeObject = EditorGUILayout.Toggle(t.gameObject.isWholeObject(), GUILayout.Width(75));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();


        if (GUI.changed)
        {
            SetAccessibilityTags();

            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }

    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            v.x = 0;
        }
        if (float.IsNaN(v.y))
        {
            v.y = 0;
        }
        if (float.IsNaN(v.z))
        {
            v.z = 0;
        }
        return v;
    }

    void OnEnable()
    {
        description = EditorPrefs.GetString("AccessDescriptioin", "");
        salience = EditorPrefs.GetBool("salience", false);
        wholeObject = EditorPrefs.GetBool("wholeObject", false);
    }


    void SetAccessibilityTags()
    {
        Transform t = (Transform)target;
        EditorPrefs.SetBool("salience", salience);
        EditorPrefs.SetBool("wholeObject", wholeObject);
        EditorPrefs.SetString("AccessDescriptioin", description);

        t.gameObject.setSalience(salience);
        t.gameObject.setWholeObject(wholeObject);
        t.gameObject.setDescription(description);

    }
}