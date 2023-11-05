using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlayCardState : IState
{
    CellSelector CellSelector { get; }

    public PlayerPlayCardState(CellSelector cellSelector)
    {
        CellSelector = cellSelector;
    }

    public void OnEnter() { CellSelector.Start(); }

    public void OnExit() { }

    public void OnUpdate() { }
}
