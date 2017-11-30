using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum State
{
    Main,
    Lobby,
    Settings,
    Game
}

public enum Command
{
    Submit,
    Back,
    Alt
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
        change(startState);

        transitions = new Dictionary<StateTransition, State> 
        {
            { new StateTransition(State.Main, Command.Submit), State.Lobby },
            { new StateTransition(State.Lobby, Command.Alt), State.Settings },
            { new StateTransition(State.Lobby, Command.Submit), State.Game }, 
            { new StateTransition(State.Settings, Command.Submit), State.Game },


            { new StateTransition(State.Game, Command.Back), State.Lobby },
            { new StateTransition(State.Settings, Command.Back), State.Lobby },
            { new StateTransition(State.Lobby, Command.Back), State.Main },
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
