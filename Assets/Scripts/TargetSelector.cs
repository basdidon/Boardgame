using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class TargetSelector
{

}

public class CellSelector : TargetSelector
{
    Grid MainGrid => BoardManager.Instance.MainGrid;
    Func<Vector3Int, bool> Predicate { get; }
    InputProvider InputProvider => Player.Instance.InputProvider;

    GameObject _focusOverlay;
    List<GameObject> predicatedOverlays;

    public CellSelector(Func<Vector3Int, bool> predicate)
    {
        Predicate = predicate;
    }

    List<Vector3Int> predicatedCells;

    //
    public void Start()
    {
        OnStart?.Invoke();
        BoardManager.Instance.StartCoroutine(GetCell());

        OnLeave += () =>
        {
            // set everything back
            if (_focusOverlay != null)
                _focusOverlay.SetActive(false);

            foreach (var overlayObject in predicatedOverlays)
            {
                overlayObject.SetActive(false);
            }
        };
    }

    public void Cancle()
    {
        isCancle = true;
       // BoardManager.Instance.StopCoroutine(Coroutine);
        OnCancle?.Invoke();
        OnLeave?.Invoke();
    }

    public void Cancle(InputAction.CallbackContext ctx)
    {
        Cancle();
    }

    public void Choose(InputAction.CallbackContext ctx)
    {
        if (TryGetCellByScreenPoint(out Vector3Int targetCell) && predicatedCells.Contains(targetCell))
        {
            //BoardManager.Instance.StopCoroutine(Coroutine);
            isPass = true;
            OnSuccess?.Invoke(targetCell);
            OnLeave?.Invoke();
            Debug.Log($"you choose : {targetCell}");
        }
    }

    bool TryGetCellByScreenPoint(out Vector3Int targetCell)
    {
        targetCell = Vector3Int.zero;

        RaycastHit[] hits = new RaycastHit[100];
        int hitsCount;
        
        hitsCount = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(InputProvider.SelectTarget.CursorPosition.ReadValue<Vector2>()), hits, 100);
        
        if (hitsCount > 0)
        {
            var slicedHits = hits.Take(hitsCount);
            if (slicedHits.Any(hit => hit.transform.CompareTag("CellTile")))
            {
                var firstCellHit = slicedHits.First(hit => hit.transform.CompareTag("CellTile"));
                targetCell = MainGrid.WorldToCell(firstCellHit.point);
                return true;
            }
        }

        return false;
    }

    // state control
    bool isPass = false;
    bool isCancle = false;

    public bool IsPass => isPass;
    public bool IsCancle => isCancle;

    // Events
    public event Action OnStart;
    public event Action<Vector3Int> OnUpdate;
    public event Action<Vector3Int> OnSuccess;
    public event Action OnCancle;
    public event Action OnLeave;

    public IEnumerator GetCell()
    {
        // Active Focus Overlay
        _focusOverlay = TileOverlayPool.Instance.GetFocusOverlay(TileOverlayPool.OverlayType.Focus);
        if(_focusOverlay != null)
            _focusOverlay.SetActive(true);

        // list of cell those meet criteria
        predicatedCells = new();
        predicatedOverlays = new();
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
            if(TryGetCellByScreenPoint(out Vector3Int targetCell))
            {
                OnUpdate?.Invoke(targetCell);

                if (_focusOverlay != null)
                    _focusOverlay.transform.position = MainGrid.GetCellCenterWorld(targetCell);
            }
               
            return isPass || isCancle;
        });

        //Debug.Log("end selection!!");
    }


}