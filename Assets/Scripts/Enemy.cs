using BasDidon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasDidon.Direction;
using BasDidon.PathFinder;

public class Enemy : Character<Enemy>
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

    public override bool TryMove(Vector3Int from, Direction dir,out Vector3Int moveResult)
    {
        throw new NotImplementedException();
    }

    protected override void OnTurnChangedHandle(ITurnRunner character)
    {
        if (!ReferenceEquals(character,this))
            return;

        Debug.Log("Enemy Turn !!");
        Debug.Log("Enemy: i am lazy, so i decided to do nothing.");
        EndTurn();
    }

    void Move()
    {
    }
}
