using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
