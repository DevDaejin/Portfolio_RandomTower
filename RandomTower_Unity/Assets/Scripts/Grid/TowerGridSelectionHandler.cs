using System;
using UnityEngine;

public static class TowerGridSelectionHandler
{
    private static ISelectable _current;

    public static void Select(ISelectable newOne)
    {
        if (_current == newOne) return;
        
        _current?.OnDeselect();
        _current = newOne;
        _current.OnSelect();
    }

    public static void Reselect()
    {
        _current?.OnSelect();
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
            if (!hit.collider.TryGetComponent<ISelectable>(out _))
                Deselect();
        }
    }

    public static void Deselect()
    {
        _current?.OnDeselect();
        _current = null;
    }
}
