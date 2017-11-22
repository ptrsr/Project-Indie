using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Points
    [System.Serializable]
    struct Points
    {
        public CameraPoint
            menuPoint,
            lobbyPoint,
            settingsPoint,
            gamePoint;
    }
    [SerializeField]
    private Points _points = new Points();
    #endregion

    [SerializeField]
    private CameraPoint _currentPoint;
    private CameraPoint _lastPoint;


    private void Awake()
    {
        ServiceLocator.Provide(this);
        StateMachine.change += CameraChange;
    }


    private void CameraChange(State newState)
    {
        _lastPoint = _currentPoint;

        switch (newState)
        {
            case State.Game:
                CurrentPoint = _points.gamePoint;
                break;

            case State.Lobby:
                CurrentPoint = _points.lobbyPoint;
                break;

            case State.Settings:
                CurrentPoint = _points.settingsPoint;
                break;

            case State.Menu:
                CurrentPoint = _points.menuPoint;
                break;
        }
    }

    private void Update()
    {
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
