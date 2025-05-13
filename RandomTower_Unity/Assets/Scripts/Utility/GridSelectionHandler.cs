using System;
using UnityEngine;

public static class GridSelectionHandler
{
    private static ISelectable _current;

    public static void Select(ISelectable newOne)
    {
        if (_current == newOne)
        {
            Deselect();
        }
        else
        {
            _current?.OnDeselect();
            _current = newOne;
            _current.OnSelect();
        }
    }

    public static void Update()
    {
        _current?.OnSelect();
    }

    public static void Deselect()
    {
        _current?.OnDeselect();
        _current = null;
    }
}
