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

    [SerializeField] [Range(0, 1)]
    private float _camMoveDist;

    [SerializeField] [Range(0, 10)]
    private float _camMoveSpeed;

    [SerializeField]
    private float _cameraMovement;

    private CameraPoint 
        _currentPoint,
        _lastPoint;

    private Vector3
        _centerOfLevel,
        _ray;

    private List<GameObject> _players;

    private bool _tracking = false;

    private void Awake()
    {
        ServiceLocator.Provide(this);
        _players = new List<GameObject>();
        StateMachine.change += CameraChange;
        StateMachine.change += TrackPlayers;

        Vector3 desiredCenterPos = _points.gamePoint.transform.position;
        _ray = _points.gamePoint.transform.rotation * Vector3.forward;
        _ray *= _points.gamePoint.transform.position.z / _ray.z;
        _centerOfLevel = desiredCenterPos - _ray;
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

        if (!_tracking || _players.Count == 0)
        {
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
            return;
        }

        Vector3 desiredLookPosition = new Vector3();

        foreach (GameObject player in _players)
            desiredLookPosition += player.transform.position;
        desiredLookPosition.y = 0;

        desiredLookPosition = Vector3.Lerp(desiredLookPosition, _centerOfLevel, _camMoveDist);

        Vector3 desiredCameraPosition = desiredLookPosition + _ray;

        transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, _camMoveSpeed * Time.deltaTime);
    }

    public CameraPoint CurrentPoint
    {
        get { return _currentPoint;  }
        set { _currentPoint = value; }
    }

    private void TrackPlayers(State state)
    {
        if (state != State.Game)
        {
            _tracking = false;
            _players.Clear();
            return;
        }

        _tracking = true;
    }

    public void Track(GameObject player)
    {
        _players.Add(player);
    }

    public void UnTrack(GameObject player)
    {
        _players.Remove(player);
    }
}
