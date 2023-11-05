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

    // state control
    public enum SelectorPhase { created,started,performed,cancled}
    public SelectorPhase Phase { get; private set; }

    // Events
    public event Action OnStart;
    public event Action<Vector3Int> OnUpdate;
    public event Action<Vector3Int> OnSuccess;
    public event Action OnCancle;
    public event Action OnLeave;

    public CellSelector(Func<Vector3Int, bool> predicate)
    {
        Predicate = predicate;
        Phase = SelectorPhase.created;
    }

    List<Vector3Int> predicatedCells;

    //
    public void Start() 
    {
        OnStart?.Invoke();
        Phase = SelectorPhase.started;

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

    public void Cancle(InputAction.CallbackContext _) => Cancle();
    public void Cancle() 
    {
        Phase = SelectorPhase.cancled;
        OnCancle?.Invoke();
        OnLeave?.Invoke();
    }

    public void Choose(InputAction.CallbackContext _)
    {
        if (TryGetCellByScreenPoint(out Vector3Int targetCell) && predicatedCells.Contains(targetCell))
        {
            Phase = SelectorPhase.performed;
            OnSuccess?.Invoke(targetCell);
            OnLeave?.Invoke();
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
                    var cellWorldPos = MainGrid.GetCellCenterWorld(cellPos);
                    var worldPos = new Vector3(cellWorldPos.x, 0.1f, cellWorldPos.z);
                    _predicatedOverlay.transform.position = worldPos;
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
                {
                    var cellWorldPos = MainGrid.GetCellCenterWorld(targetCell);
                    var worldPos = new Vector3(cellWorldPos.x, 0.1f,cellWorldPos.z);
                    _focusOverlay.transform.position = worldPos;
                }
            }
            return Phase == SelectorPhase.performed || Phase == SelectorPhase.cancled;
               
        });
    }


}