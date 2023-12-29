using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public interface ITurnRunner
{

}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // Queue
    [SerializeField] List<ITurnRunner> queue;
    public IReadOnlyList<ITurnRunner> Queue => queue;

    // CurruntTurn
    [SerializeField]  ITurnRunner currentTurn;
    public ITurnRunner CurrentTurn {
        get => currentTurn;
        private set
        {
            currentTurn = value;
            OnTurnChanged?.Invoke(CurrentTurn);
        }
    }
    public event Action<ITurnRunner> OnTurnChanged;

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

        CardFactory.Initialize();
    }

    private void Start()
    {
        if(Queue.Count > 0)
        {
            CurrentTurn = Queue[0];
        }
    }

    public void TurnRegister(ITurnRunner turnRunner)
    {
        queue.Add(turnRunner);
    }

    public void TurnUnregister(ITurnRunner turnRunner)
    {
        queue.Remove(turnRunner);
        if (CurrentTurn == turnRunner && Queue.Count > 0)
        {
            CurrentTurn = Queue[0];
        }
    }

    public void EndTurn(ITurnRunner turnRunner)
    {
        if (CurrentTurn != turnRunner)
            return;

        queue.RemoveAt(0);
        queue.Add(CurrentTurn);
        CurrentTurn = Queue[0];
    }

    public void NextTurn()
    {
        EndTurn(CurrentTurn);
    }
}

[CustomEditor(typeof(TurnManager))]
public class TurnManagerCustomEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var scriptRefOF = new ObjectField()
        {
            label = "Script",
            bindingPath = "m_Script"
        };
        scriptRefOF.SetEnabled(false);

        if (Application.isPlaying)
        {
            var btn = new Button()
            {
                text = "End Turn"
            };

            btn.clicked += OnEndturnBtnClicked;

            container.Add(btn);
        }

        container.Insert(0, scriptRefOF);
        return container;
    }

    private void OnEndturnBtnClicked()
    {
        if (target is TurnManager turnManager)
        {
            turnManager.NextTurn();
        }
    }
}