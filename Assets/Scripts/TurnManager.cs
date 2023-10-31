using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // Queue
    [SerializeField] List<Character> queue;
    public IReadOnlyList<Character> Queue => queue;

    // CurruntTurn
    [SerializeField]  Character currentTurn;
    public Character CurrentTurn {
        get => currentTurn;
        private set
        {
            currentTurn = value;
            CurrentActionPoint = maxActionPoint;
            OnTurnChanged?.Invoke(CurrentTurn);
        }
    }
    public Action<Character> OnTurnChanged;

    // action point
    readonly int maxActionPoint = 3;
    public int CurrentActionPoint { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        queue = new();
    }

    private void Start()
    {
        if(Queue.Count > 0)
        {
            CurrentTurn = Queue[0];
        }

        Debug.Log("Start");
    }

    public void TurnRegister(Character character)
    {
        queue.Add(character);
    }

    public void TurnUnregister(Character character)
    {
        queue.Remove(character);
        if (CurrentTurn == character && Queue.Count > 0)
        {
            CurrentTurn = Queue[0];
        }
    }

    public void EndTurn(Character character)
    {
        if (CurrentTurn != character)
            return;

        queue.RemoveAt(0);
        queue.Add(CurrentTurn);
        CurrentTurn = Queue[0];
    }
}