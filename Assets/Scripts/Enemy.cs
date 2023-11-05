using BasDidon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BasDidon.Direction;

public class Enemy : Character
{
    public override bool CanMoveTo(Vector3Int cellPos) => false;

    public override void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0) {
            BoardManager.Instance.RemoveBoardObject(this);
            Destroy(gameObject);
        }
    }

    public override bool TryMove(Vector3Int from, Directions dir,out Vector3Int moveResult)
    {
        throw new NotImplementedException();
    }

    protected override void OnTurnChangedHandle(Character character)
    {
        if (character != this)
            return;

        Debug.Log("Enemy Turn !!");
        Debug.Log("Enemy: i am lazy, so i decide to do nothing.");
        EndTurn();
    }
}
