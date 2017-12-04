using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : Editor
{
    SerializedProperty _setting;
    SerializedProperty _value;


    void OnEnable()
    {
        _setting = serializedObject.FindProperty("_setting");
        _value = serializedObject.FindProperty("_value");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_setting);
        EditorGUILayout.PropertyField(_value);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif