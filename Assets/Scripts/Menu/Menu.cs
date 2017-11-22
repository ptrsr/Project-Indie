using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private StateMachine _sm = new StateMachine(State.Menu);

    private void Awake()
    {
        ServiceLocator.Provide(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            _sm.MoveNext(Command.Continue);

        if (Input.GetKeyDown(KeyCode.Q))
            _sm.MoveNext(Command.Exit);
    }

    public State CurrentState
    {
        get { return _sm.CurrentState; }
    }
}
