using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void Awake()
    {
        base.Awake();
        TurnManager.Instance.OnTurnChanged += OnTurnChangedHandle;
    }

    private void OnTurnChangedHandle(Character character)
    {
        if (character != this)
            return;
        Debug.Log("Enemy Turn !!");
        Debug.Log("Enemy: i am lazy, so i decide to do nothing.");
        EndTurn();
    }
}
