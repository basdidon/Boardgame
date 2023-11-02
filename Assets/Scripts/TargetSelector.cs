using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSelector
{

}

public class CellSelector : TargetSelector
{
    Grid MainGrid => BoardManager.Instance.MainGrid;
    Func<Vector3Int, bool> Predicate { get; }
    readonly InputProvider inputProvider;

    public CellSelector(Func<Vector3Int, bool> predicate)
    {
        Predicate = predicate;
        inputProvider = Player.Instance.GetComponent<InputProvider>();
    }

    Vector3Int targetCell = Vector3Int.zero;
    List<Vector3Int> predicatedCells;

    //
    public void Start() => BoardManager.Instance.StartCoroutine(GetCell());
    public void Cancle() => isCancle = true;
    public void Choose() => isPass = predicatedCells.Contains(targetCell);

    // state control
    bool isPass = false;
    bool isCancle = false;

    // Events
    public Action<Vector3Int> OnSuccess { get; set; }
    public Action<Vector3Int> OnUpdate { get; set; }
    public Action OnCancle { get; set; }

    public IEnumerator GetCell()
    {
        RaycastHit[] hits = new RaycastHit[100];
        int hitsCount;
        Debug.Log("StartSelection");

        // Active Focus Overlay
        var _focusOverlay = TileOverlayPool.Instance.GetFocusOverlay(TileOverlayPool.OverlayType.Focus);
        if (_focusOverlay != null)
            _focusOverlay.SetActive(true);

        // list of cell those meet criteria
        predicatedCells = new();
        List<GameObject> predicatedOverlays = new();
        foreach (var rectPos in BoardManager.Instance.MapRect.allPositionsWithin)
        {
            var cellPos = (Vector3Int)rectPos;
            if (Predicate(cellPos))
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

            if (targetCell != null)
                OnUpdate?.Invoke(targetCell);

            return isPass || isCancle;
        });

        // set everything back
        if (_focusOverlay != null)
            _focusOverlay.SetActive(false);

        foreach (var overlayObject in predicatedOverlays)
        {
            overlayObject.SetActive(false);
        }

        if (!isCancle)
        {
            OnSuccess?.Invoke(targetCell);
        }
        else
        {
            OnCancle?.Invoke();
        }


        Debug.Log("end selection!!");
    }
}