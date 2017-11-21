using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private StateMachine _sm = new StateMachine(State.Menu);

    [SerializeField]
    private CameraPoint
        _menuPoint,
        _lobbyPoint,
        _settingsPoint,
        _gamePoint;

    private Transform
        _camera;

    private void Awake()
    {
        ServiceLocator.Provide(this);
        _camera = Camera.main.transform;
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public State CurrentState
    {
        get { return _sm.CurrentState; }
    }
}
