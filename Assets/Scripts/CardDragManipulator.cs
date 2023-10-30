using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardDragManipulator : PointerManipulator
{
    private VisualElement Root { get; }
    private Vector2 TargetStartPosition { get; set; }
    private Vector3 PointerStartPosition { get; set; }
    private bool Enabled { get; set; }

    public CardDragManipulator(VisualElement target, VisualElement root)
    {
        this.target = target;
        Root = root;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        TargetStartPosition = target.transform.position;
        PointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        Enabled = true;
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (Enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.transform.position = evt.position - PointerStartPosition;
        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        if (Enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (Enabled)
        {
            VisualElement playZone = Root.Q("play-a-card-panel");
            Debug.Log($"{playZone.worldBound} : {target.worldBound}");
            if (target.worldBound.Overlaps(playZone.worldBound))
            {
                Debug.Log("on drop-zone");
                if (target.userData is Card card)
                {
                    card.UseCard();
                }
            }
            /*
            VisualElement slotsContainer = Root.Q<VisualElement>("slots");
            UQueryBuilder<VisualElement> allSlots = slotsContainer.Query<VisualElement>(className: "slot");
            UQueryBuilder<VisualElement> overlappingSlots = allSlots.Where(OverlapsTarget);
            VisualElement closestOverlappingSlot = FindClosestSlot(overlappingSlots);
            Vector3 closestPos = Vector3.zero;
            if (closestOverlappingSlot != null)
            {
                closestPos = RootSpaceOfSlot(closestOverlappingSlot);
                closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5);
            }
            target.transform.position =
                closestOverlappingSlot != null ?
                closestPos :
                TargetStartPosition;
            */
            target.transform.position = TargetStartPosition;
            Enabled = false;
        }
    }

    private bool OverlapsTarget(VisualElement slot)
    {
        return target.worldBound.Overlaps(slot.worldBound);
    }

    private VisualElement FindClosestSlot(UQueryBuilder<VisualElement> slots)
    {
        List<VisualElement> slotsList = slots.ToList();
        float bestDistanceSq = float.MaxValue;
        VisualElement closest = null;
        foreach (VisualElement slot in slotsList)
        {
            Vector3 displacement = RootSpaceOfSlot(slot) - target.transform.position;
            float distanceSq = displacement.sqrMagnitude;
            if (distanceSq < bestDistanceSq)
            {
                bestDistanceSq = distanceSq;
                closest = slot;
            }
        }
        return closest;
    }

    private Vector3 RootSpaceOfSlot(VisualElement slot)
    {
        Vector2 slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
        return Root.WorldToLocal(slotWorldSpace);
    }
}