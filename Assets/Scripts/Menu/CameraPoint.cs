using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPoint : MonoBehaviour
{
    [SerializeField]
    private float
        _departSpeed,
        _arriveSpeed;


    public float DepartSpeed
    {
        get { return _departSpeed; }
    }

    public float ArriveSpeed
    {
        get { return _arriveSpeed; }
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        CameraMovement movement = ServiceLocator.Locate<CameraMovement>();

        if (movement == null || !movement.transform.hasChanged)
            return;

        if (movement.CurrentPoint == this)
        {
            movement.transform.position = transform.position;
            movement.transform.rotation = transform.rotation;
        }
    }
}
