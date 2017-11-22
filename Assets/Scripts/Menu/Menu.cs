using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : EventSystem
{
    [SerializeField]
    private State _startState;

    private StateMachine _sm;

    [SerializeField]
    private CustomInputHandler _handler;

    private GameObject _lastSelectedObject;

    protected override void Awake()
    {
        base.Awake();

        _sm = new StateMachine(_startState);
        ServiceLocator.Provide(this);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E))
            _sm.MoveNext(Command.Continue);

        if (Input.GetKeyDown(KeyCode.Q))
            _sm.MoveNext(Command.Exit);
    }

    //public void Hover(GameObject obj)
    //{
    //    print(obj.name);
    //    EventTrigger trigger;
    //    _system.
    //    trigger.on
    //}

    private void OnGUI()
    {
        if (currentSelectedGameObject != null)
            _lastSelectedObject = currentSelectedGameObject;

        List<RaycastResult> results = new List<RaycastResult>();
        RaycastAll(_handler.GetLastPointerEventDataPublic(), results);

        if (results.Count != 0 && results[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>().gameObject != currentSelectedGameObject)
            SetSelectedGameObject(null);
        else
            SetSelectedGameObject(_lastSelectedObject);
    }

    public void SendCommand(int command)
    {
        _sm.MoveNext((Command)command);
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        CameraMovement camMovement = ServiceLocator.Locate<CameraMovement>();

        if (camMovement != null)
            camMovement.CameraChange(_startState);
    }

    public State CurrentState
    {
        get { return _sm.CurrentState; }
    }
}
