using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using BasDidon.Direction;
using DG.Tweening;
using System;
using Object = UnityEngine.Object;

public class FireBallCard : Card
{
    public override string CardSOName => "FireBall";
    string ProjectilePrefabURL => "ProjectilePrefabs/FireballProjectile";

    readonly int distance = 5;

    public override void UseCard()
    {
        if (TurnManager.Instance.CurrentTurn != Player)
            return;

        if (Player.State != Player.IdleState)
            return;

        Debug.Log("use Card");

        CardVE.style.display = DisplayStyle.None;

        /// this card required empty space on adjucent tlie in direction that that player choose
        /// on which direction that get blocks, i don't let player select that direction
        DirectionGroup selectableDir = new();
        foreach (var dir in DirectionGroup.CardinalDirection)
        {
            if (!BoardManager.Instance.BoardObjectsOnCell(Player.CellPos + dir.DirectionVector).Any()) // if nothing on cell
            {
                selectableDir.Add(dir); // add direction to selectableDir
            }
        }

        CellSelector cellSelector = new((cell)=> {
            var result = selectableDir.IsCellInDirectionGroup(Player.CellPos, cell);
            if (result)
            {
                Debug.Log($"{cell} - {Player.CellPos} : {cell - Player.CellPos}");
            }
            return result;
        });
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

            if(cellSelector.Phase == CellSelector.SelectorPhase.cancled)
            {
                Player.State = null;
            }
        };
        Player.State = new PlayerPlayCardState(cellSelector);
    }

    public void Execute(Player player, Vector3Int targetCell)
    {
        Debug.Log($"FireBall !!! on {targetCell}");
        Debug.Log($"spend mana : {CardSO.Cost}");

        Vector3Int direction = targetCell - player.CellPos;
        Vector3Int normalizeDirection;

        if (direction.x < 0)
        {
            normalizeDirection = Vector3Int.left;
        }
        else if (direction.x > 0)
        {
            normalizeDirection = Vector3Int.right;
        }
        else if (direction.y > 0)
        {
            normalizeDirection = Vector3Int.up;
        }
        else if (direction.y < 0)
        {
            normalizeDirection = Vector3Int.down;
        }
        else
        {
            throw new System.Exception();
        }


        var prefab = Resources.Load(ProjectilePrefabURL) as GameObject;
        var obj = Object.Instantiate(prefab, BoardManager.Instance.MainGrid.GetCellCenterWorld(player.CellPos + normalizeDirection), Quaternion.identity);
        // predict projectile what it gonna hit

        var stopAt = distance;
        Enemy hitEnemy = null;

        for(int i = 1; i <= distance; i++)
        {
            var objs = BoardManager.Instance.BoardObjectsOnCell(player.CellPos + normalizeDirection * i);

            if (!objs.Any()) // empty
                continue;

            if (objs.Any(obj => obj.Transform.CompareTag("Enemy")))
            {
                stopAt = i;

                // battle logic go below
                if(objs.First(obj => obj.Transform.CompareTag("Enemy")) is Enemy enemy)
                {
                    hitEnemy = enemy;
                }

                break;
            }

            if (objs.Any(obj => obj.Transform.CompareTag("Obstacle")))
            {
                stopAt = i;
                break;
            }
        }

        Sequence mySequence = DOTween.Sequence();
        mySequence.PrependInterval(.5f);
        mySequence.Append(obj.transform.DOMove(BoardManager.Instance.MainGrid.GetCellCenterWorld(player.CellPos + normalizeDirection * stopAt), .1f * stopAt));
        mySequence.onComplete += () => {
            if (hitEnemy != null)
                hitEnemy.TakeDamage(5);
            Object.Destroy(obj);
            player.State = null;
        };
    }
}