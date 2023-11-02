using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using BasDidon;
using DG.Tweening;

public class FireBallCard : Card
{
    public override string CardSOName => "FireBallCardData";
    string ProjectilePrefabURL => "ProjectilePrefabs/FireballProjectile";

    readonly int distance = 5;

    public override void UseCard()
    {
        if (TurnManager.Instance.CurrentTurn != Player)
            return;

        CardVE.style.display = DisplayStyle.None;

        /// this card required empty space on adjucent tlie in direction that that player choose
        /// on which direction that get blocks, i don't let player select that direction
        Direction.Directions selectableDir = Direction.Directions.None;
        foreach (var dir in Direction.CardinalVector)
        {
            if (!BoardManager.Instance.BoardObjectsOnCell(Player.CellPos + dir).Any()) // if nothing on cell
            {
                selectableDir |= Direction.Vector3IntToDirection(dir); // add direction to selectableDir
            }
        }

        Player.StartCoroutine(PlayerSelector.Instance.GetCell(
            cell => Direction.IsCellInDirection(Player.CellPos, cell, selectableDir),
            OnSuccess: cell => Execute(Player, cell),
            OnCancle: () => CardVE.style.display = DisplayStyle.Flex   // when cancle make card display again
        ));
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
                    Debug.Log("-----------------");
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


        };
    }
}