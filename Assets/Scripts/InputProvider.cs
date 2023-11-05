using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class InputProvider : MonoBehaviour
{
    public Inputs PlayerInputs { get; private set; }
    Player Player { get; set; }

    // Action Map
    public Inputs.BaseGameplayActions BaseGameplay { get; private set; }
    public Inputs.SelectTargetActions SelectTarget { get; private set; }

    private void Awake()
    {
        PlayerInputs = new Inputs();
        Player = GetComponent<Player>();

        BaseGameplay = PlayerInputs.BaseGameplay;
        SelectTarget = PlayerInputs.SelectTarget;

        BaseGameplay.Movement.performed += ctx =>
        {
            var value = ctx.ReadValue<Vector2>();

            if (value == Vector2.zero)
                return;

            Vector2Int dir;

            if (value.x > 0)
            {
                dir = Vector2Int.right;
            }
            else if (value.x < 0)
            {
                dir = Vector2Int.left;
            }
            else if (value.y > 0)
            {
                dir = Vector2Int.up;
            }
            else //if (value.y < 0)
            {
                dir = Vector2Int.down;
            }

            Debug.Log($"input {dir}");

            Player.Move((Vector3Int)dir);
        };

        BaseGameplay.Roll.performed += _ => {};

        BaseGameplay.EndTurn.performed += _ => Player.EndTurn();
    }

    public void OnEnable()
    {
        BaseGameplay.Enable();
    }

    public void OnDisable()
    {
        BaseGameplay.Disable();
    }
}