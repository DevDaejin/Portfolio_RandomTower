using System;
using UnityEngine;

public static class TowerGridSelectionHandler
{
    public static TowerGrid Current => _current;
    private static TowerGrid _current;

    public static Action<TowerGrid> OnSelect;
    public static Action OnDeselect;

    public static void Select(TowerGrid newOne)
    {
        if (Current == newOne) return;

        Deselect();
        _current = newOne;
        Select();
    }

    public static void Select()
    {
        if (Current == null) return;

        OnDeselect?.Invoke();
        Current?.OnSelect();
        OnSelect?.Invoke(Current);
    }

    public static void Deselect()
    {
        if(Current == null) return;

        OnDeselect?.Invoke();
        Current?.OnDeselect();
        _current = null;
    }

    public static void TryDeselectOnEmptyClick(Vector3 screenPosition)
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            Deselect();
        }
        else
        {
            if (!hit.collider.TryGetComponent<TowerGrid>(out _))
            {
                Deselect();
            }
        }
    }

    public static void Clear() => _current = null;
}
