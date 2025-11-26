using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    private Dictionary<string, IState> states = new Dictionary<string, IState>();
    private IState currentState;

    public void AddState(string stateName, IState state)
    {
        states[stateName] = state;
    }

    public void ChangeState(string stateName)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        if (states.ContainsKey(stateName))
        {
            currentState = states[stateName];
            currentState.Enter();
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public string GetCurrentStateName()
    {
        return currentState?.GetType().Name;
    }
}