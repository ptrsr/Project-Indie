#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BulletTracker))]
public class TrackerInspector : Editor
{
    private bool _visualize = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string text = _visualize ? "Hide" : "Show";

        if (GUILayout.Button(text + " trajectories"))
        {
            _visualize ^= true;
            ((BulletTracker)target).Visualize = _visualize;
        }
    }
}
#endif
