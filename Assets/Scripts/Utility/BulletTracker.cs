#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracker : MonoBehaviour
{
    private static List<List<Vector3>> _trajectories = new List<List<Vector3>>();

    private bool _visualize = false;

    private void Awake()
    {
        ServiceLocator.Provide(this);
    }

    public void AddTrajectory(List<Vector3> bounces)
    {
        _trajectories.Add(bounces);
    }

    void Start ()
    {
        _trajectories.Clear();
	}

    private void OnDrawGizmos()
    {
        if (!_visualize)
            return;

        for (int i = 0; i < _trajectories.Count; i++)
        {
            List<Vector3> bounces = _trajectories[i];

            for (int j = 0; j < bounces.Count - 1; j++)
                Gizmos.DrawLine(bounces[j], bounces[j + 1]);
        }
    }

    public bool Visualize
    {
        set { _visualize = value; }
    }
}
#endif
