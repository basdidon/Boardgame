using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

public class PlayerSelector : MonoBehaviour
{
    public static PlayerSelector Instance { get; private set; }

    TurnManager TurnManager => TurnManager.Instance;
    Grid MainGrid => BoardManager.Instance.MainGrid;

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
    }

    public IEnumerator GetCell(Func<Vector3Int, bool> predicate, Action<Vector3Int> OnSuccess = null, Action OnCancle = null)
    {
        bool isPass = false;
        bool isCancle = false;
        Vector3Int targetCell = Vector3Int.zero;
        RaycastHit[] hits = new RaycastHit[100];
        int hitsCount;
        Debug.Log("StartSelection");

        // Active Focus Overlay
        var _focusOverlay = TileOverlayPool.Instance.GetFocusOverlay(TileOverlayPool.OverlayType.Focus);
        if (_focusOverlay != null)
            _focusOverlay.SetActive(true);

        // list of cell those meet criteria
        List<Vector3Int> predicatedCells = new();
        List<GameObject> predicatedOverlays = new();
        foreach (var rectPos in BoardManager.Instance.MapRect.allPositionsWithin)
        {
            var cellPos = (Vector3Int)rectPos;
            if (predicate(cellPos))
            {
                predicatedCells.Add(cellPos);
                var _predicatedOverlay = TileOverlayPool.Instance.GetFocusOverlay(TileOverlayPool.OverlayType.Green);
                if (_predicatedOverlay != null)
                {
                    _predicatedOverlay.SetActive(true);
                    _predicatedOverlay.transform.position = MainGrid.GetCellCenterWorld(cellPos);
                }
                predicatedOverlays.Add(_predicatedOverlay);
            }
        }

        if (predicatedCells.Count <= 0)
        {
            Debug.Log("there is no cell meet condition");
            yield return null;
        }

        // Input Logic
        InputProvider inputProvider = Player.Instance.GetComponent<InputProvider>();

        inputProvider.BaseGameplay.Disable();
        inputProvider.SelectTarget.Enable();
        inputProvider.SelectTarget.LeftClick.performed += _ => isPass = OnPickCell(targetCell, predicatedCells);
        inputProvider.SelectTarget.Cancle.performed += _ => isCancle = true;

        yield return new WaitUntil(() => {
            hitsCount = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(inputProvider.SelectTarget.CursorPosition.ReadValue<Vector2>()), hits, 100);
            if (hitsCount > 0)
            {
                var slicedHits = hits.Take(hitsCount);
                if (slicedHits.Any(hit => hit.transform.CompareTag("CellTile")))
                {
                    var firstCellHit = slicedHits.First(hit => hit.transform.CompareTag("CellTile"));
                    targetCell = MainGrid.WorldToCell(firstCellHit.point);

                    if (_focusOverlay != null)
                        _focusOverlay.transform.position = MainGrid.GetCellCenterWorld(targetCell);
                }
            }

            return isPass || isCancle;
        });

        if (!isCancle)
        {
            OnSuccess(targetCell);
        }
        else
        {
            OnCancle();
        }

        // set everything back
        if (_focusOverlay != null)
            _focusOverlay.SetActive(false);

        foreach (var overlayObject in predicatedOverlays)
        {
            overlayObject.SetActive(false);
        }

        inputProvider.SelectTarget.LeftClick.performed -= _ => isPass = OnPickCell(targetCell, predicatedCells);
        inputProvider.SelectTarget.Cancle.performed -= _ => isCancle = true;
        inputProvider.SelectTarget.Disable();
        inputProvider.BaseGameplay.Enable();
        Debug.Log("end selection!!");
    }

    bool OnPickCell(Vector3Int targetCell, List<Vector3Int> predicatedCells)
    {
        return predicatedCells.Contains(targetCell);
    }
}