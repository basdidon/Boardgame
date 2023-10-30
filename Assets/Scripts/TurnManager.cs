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
    [SerializeField] List<Player> playersQueue;
    public IReadOnlyList<Player> PlayersQueue
    {
        get => playersQueue;
    }

    // CurruntTurn
    [SerializeField]  Player currentPlayer;
    public Player CurrentTurn {
        get => currentPlayer;
        private set
        {
            currentPlayer = value;
            CurrentActionPoint = maxActionPoint;
            OnTurnChanged?.Invoke(CurrentTurn);
        }
    }
    public Action<Player> OnTurnChanged;

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

        playersQueue = new();
    }

    public void TurnRegister(Player player)
    {
        playersQueue.Add(player);
        StartRunTurn();
    }

    public void TurnUnregister(Player player)
    {
        playersQueue.Remove(player);
        if (CurrentTurn == player && PlayersQueue.Count > 0)
        {
            CurrentTurn = PlayersQueue[0];
        }
    }

    public void StartRunTurn()
    {
        CurrentTurn = PlayersQueue[0];
    }

    public void NextTurn()
    {
        playersQueue.RemoveAt(0);
        playersQueue.Add(CurrentTurn);
        CurrentTurn = PlayersQueue[0];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TurnManager))]
public class TurnManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (target is TurnManager _target)
        {
            if (Application.isPlaying)
            {
                if (GUILayout.Button("StartRunTurn"))
                {
                    _target.StartRunTurn();
                }
                if (GUILayout.Button("NextTurn"))
                {
                    _target.NextTurn();
                }
            }
        }

    }
}
#endif