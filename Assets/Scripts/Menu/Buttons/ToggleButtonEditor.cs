using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
		DrawDefaultInspector ();
    }
}
#endif