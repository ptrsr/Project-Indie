using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraMovement : MonoBehaviour
{
    #region Points
    [System.Serializable]
    struct Points
    {
        public CameraPoint
            mainPoint,
            lobbyPoint,
            settingsPoint,
            gamePoint;
    }
    [SerializeField]
    private Points _points = new Points();
    #endregion

    private CameraPoint 
        _currentPoint,
        _lastPoint;


    private void Awake()
    {
        StateMachine.change += CameraChange;
    }

    private void OnEnable()
    {
        ServiceLocator.Provide(this);
    }

    public void CameraChange(State newState)
    {
        _lastPoint = _currentPoint;

        switch (newState)
        {
            case State.Main:
                CurrentPoint = _points.mainPoint;
                break;

            case State.Lobby:
                CurrentPoint = _points.lobbyPoint;
                break;

            case State.Settings:
                CurrentPoint = _points.settingsPoint;
                break;

            case State.Game:
                CurrentPoint = _points.settingsPoint; //Trying out using same as settings
                break;
        }

        if (!Application.isPlaying)
        {
            transform.position = _currentPoint.transform.position;
            transform.rotation = _currentPoint.transform.rotation;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        float speed;

        if (_lastPoint != null)
        {
            float camDistToPoint = Vector3.Distance(transform.position, _currentPoint.transform.position);
            float distBetweenPoints = Vector3.Distance(_lastPoint.transform.position, _currentPoint.transform.position);
            speed = Mathf.Lerp(_currentPoint.ArriveSpeed, _lastPoint.DepartSpeed, camDistToPoint / distBetweenPoints) * Time.deltaTime;
        }
        else
            speed = _currentPoint.ArriveSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, _currentPoint.transform.rotation, speed);
        transform.position = Vector3.Lerp(transform.position, _currentPoint.transform.position, speed);
    }

    public CameraPoint CurrentPoint
    {
        get { return _currentPoint;  }
        set { _currentPoint = value; }
    }
}
