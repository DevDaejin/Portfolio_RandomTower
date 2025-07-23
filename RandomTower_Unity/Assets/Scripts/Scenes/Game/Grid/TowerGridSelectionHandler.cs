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
        _current = null;
    }

    public static void Clear() => _current = null;
}
