using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using DG.Tweening;
using Object = UnityEngine.Object;

public class Teleport : Card
{
    public override string CardSOName => "Teleport";
    string ProtalPrefabURL => "Portals/VortexPortalBlue";

    public override void UseCard()
    {
        if (TurnManager.Instance.CurrentTurn != Player)
            return;

        if (Player.State != Player.IdleState)
            return;

        CardVE.style.display = DisplayStyle.None;

        CellSelector cellSelector = new((cell)=> !BoardManager.Instance.BoardObjectsOnCell(cell).Any());
        cellSelector.OnStart += () =>
        {
            Player.InputProvider.SelectTarget.Enable();
            Player.InputProvider.SelectTarget.LeftClick.performed += cellSelector.Choose;
            Player.InputProvider.SelectTarget.Cancle.performed += cellSelector.Cancle;
        };
        cellSelector.OnSuccess += cell => Execute(Player, cell);
        cellSelector.OnCancle += () =>
        {
            CardVE.style.display = DisplayStyle.Flex;   // when cancle make card display again
        };
        cellSelector.OnLeave += () => {
            Player.InputProvider.SelectTarget.LeftClick.performed -= cellSelector.Choose;
            Player.InputProvider.SelectTarget.Cancle.performed -= cellSelector.Cancle;
            Player.InputProvider.SelectTarget.Disable();

            if (cellSelector.Phase == CellSelector.SelectorPhase.cancled)
            {
                Player.State = null;
            }
        };

        Player.State = new PlayerPlayCardState(Player,cellSelector);
    }

    private void Execute(Player player, Vector3Int cell)
    {

        var prefab = Resources.Load(ProtalPrefabURL) as GameObject;
        var portal_1 = Object.Instantiate(prefab, BoardManager.Instance.MainGrid.GetCellCenterWorld(player.CellPos) + Vector3.down * .99f, Quaternion.identity);
        var portal_2 = Object.Instantiate(prefab, BoardManager.Instance.MainGrid.GetCellCenterWorld(cell) + Vector3.down * .99f, Quaternion.identity);

        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(.5f);
        mySequence.Append(player.Transform.DOMoveY(-3, 1f).OnStepComplete(() => player.transform.position = BoardManager.Instance.MainGrid.GetCellCenterWorld(cell) + Vector3.down * 5));
        mySequence.Join(player.Transform.DORotate(new Vector3(0, 360*3, 0), 1f,RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear));
        mySequence.Append(player.Transform.DOMoveY(1, 1f));
        mySequence.Join(player.Transform.DORotate(new Vector3(0, 360*3, 0), 1f,RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear));
        mySequence.AppendInterval(.5f);

        //mySequence.Append(player.Transform.DOMoveY(-5, 1f));

        mySequence.OnComplete(() =>
        {
            player.CellPos = cell;
            player.State = null;
            Object.Destroy(portal_1);
            Object.Destroy(portal_2);
        });
    }

    void Callback()
    {

    }
}
