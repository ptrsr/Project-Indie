using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InputHandler))]
public class Menu : EventSystem
{
    [SerializeField]
    private State _startState;

    private StateMachine _sm;

    private InputHandler _handler;

    private GameObject _lastSelectedObject;

    [SerializeField]
    private SubMenu
        _main,
        _lobby,
        _settings;

    private SubMenu _currentMenu;

    protected override void Awake()
    {
        base.Awake();
        _handler = GetComponent<InputHandler>();
        ServiceLocator.Provide(this);
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.change += ChangeMenu;
        _sm = new StateMachine(_startState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
            _sm.MoveNext(Command.Continue);

        if (Input.GetKeyDown(KeyCode.Q))
            _sm.MoveNext(Command.Exit);
    }

    private void OnGUI()
    {
        CheckMenu();
    }

	#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        CameraMovement camMovement = ServiceLocator.Locate<CameraMovement>();

        if (camMovement != null)
            camMovement.CameraChange(_startState);
    }
	#endif

    private void CheckMenu()
    {
        if (currentSelectedGameObject != null)
            _lastSelectedObject = currentSelectedGameObject;

        List<RaycastResult> results = new List<RaycastResult>();
        RaycastAll(_handler.GetLastPointerEventDataPublic(), results);

        if (results.Count != 0)
        {
            var button = results[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>();

            if (button != null && button.gameObject != currentSelectedGameObject)
            {
                SetSelectedGameObject(null);
                return;
            }
        }
        SetSelectedGameObject(_lastSelectedObject);
    }

    private void ChangeMenu(State newState)
    {
        if (_currentMenu != null)
            _currentMenu.DisableMenu();

        switch(newState)
        {
            case State.Main:
                _currentMenu = _main;
                break;

            case State.Lobby:
                _currentMenu = _lobby;
                break;

            case State.Settings:
                _currentMenu = _settings;
                break;
        }

        if (_currentMenu != null)
            SetSelectedGameObject(_currentMenu.EnableMenu());
    }

    public void SendCommand(int command)
    {
        _sm.MoveNext((Command)command);
    }

    public State CurrentState
    {
        get { return _sm.CurrentState; }
    }
}
