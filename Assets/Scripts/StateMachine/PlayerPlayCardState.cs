using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlayCardState : IState,ISelfExitState
{
    Player StateActor { get; }
    CellSelector CellSelector { get; }
    IState NextState { get; }

    public PlayerPlayCardState(Player stateActor,CellSelector cellSelector,IState nextState)
    {
        StateActor = stateActor;
        CellSelector = cellSelector;
        NextState = nextState;

        CellSelector.OnLeave += () => SetNextState();
    }

    public void OnEnter()
    {
        Debug.Log("OnPlayACard");
        CellSelector.Start();
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        
    }

    public void SetNextState()
    {
        StateActor.State = CellSelector.IsPass ? NextState : null;
    }
}
