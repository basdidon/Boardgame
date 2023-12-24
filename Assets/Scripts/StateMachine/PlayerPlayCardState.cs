using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlayCardState : IState<Player>
{
    public Player StateActor { get; }
    CellSelector CellSelector { get; }

    public PlayerPlayCardState(Player player,CellSelector cellSelector)
    {
        StateActor = player;
        CellSelector = cellSelector;
    }

    public void StartState()
    {
        CellSelector.Start();
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
    }
}
