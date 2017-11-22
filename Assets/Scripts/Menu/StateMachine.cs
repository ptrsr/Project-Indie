using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum State
{
    Menu,
    Lobby,
    Settings,
    Game
}

public enum Command
{
    Continue,
    Exit
}

public class StateMachine
{
    public delegate void Change(State state);
    public static event Change change;

    class StateTransition
    {
        readonly State CurrentState;
        readonly Command Command;

        public StateTransition(State currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    Dictionary<StateTransition, State> transitions;
    public State CurrentState { get; private set; }

    public StateMachine(State startState)
    {
        CurrentState = startState;

        transitions = new Dictionary<StateTransition, State> 
        {
            { new StateTransition(State.Menu, Command.Continue), State.Lobby },
            { new StateTransition(State.Lobby, Command.Continue), State.Settings },
            { new StateTransition(State.Settings, Command.Continue), State.Game },


            { new StateTransition(State.Game, Command.Exit), State.Lobby },
            { new StateTransition(State.Settings, Command.Exit), State.Lobby },
            { new StateTransition(State.Lobby, Command.Exit), State.Menu },
        };
    }

    private State GetNext(Command command)
    {
        StateTransition transition = new StateTransition(CurrentState, command);
        State NextState;

        if (!transitions.TryGetValue(transition, out NextState))
        {
            Debug.LogWarning("Invalid Transition: " + CurrentState + " -> " + command);
            return CurrentState;
        }
        return NextState;
    }

    public State MoveNext(Command command)
    {
        State nextState = GetNext(command);

        if (nextState != CurrentState)
            change(nextState);

        CurrentState = nextState;
        return CurrentState;
    }
}
