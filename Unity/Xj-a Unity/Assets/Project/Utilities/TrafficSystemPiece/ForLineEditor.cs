using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ForLine))]
public class ForLineEditor : Editor
{
    // Start is called before the first frame update

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ForLine line = (ForLine)target;
        if (GUILayout.Button("Get name by end"))
        {
            string text = line.GetNameOfEndNode();
            Debug.Log(text);
        }
    }
}
