using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grid : MonoBehaviour, IPointerDownHandler, ISelectable
{
    private Transform _transform;
    private List<ITower> _towers;
    private const int MaxCount = 3;

    private const float LeftSideX = -0.2f;
    private const float RightSideX = 0.2f;
    private const float intervalZ = 0.2f;

    private static readonly Vector3[] Positions3 = new[]
    {
        new Vector3(LeftSideX, 0, intervalZ),
        new Vector3(RightSideX, 0, 0),
        new Vector3(LeftSideX, 0, -intervalZ),
    };

    private static readonly Vector3[] Positions2 = new[]
    {
        new Vector3(LeftSideX, 0, intervalZ * 0.5f),
        new Vector3(RightSideX, 0, -intervalZ * 0.5f),
    };

    public void Initialize()
    {
        _transform = GetComponent<Transform>();
        _towers = new();
    }

    public bool TryAddTower(ITower tower)
    {
        ITower exist = 
            _towers
            .FirstOrDefault(createdTower => createdTower.Data.ID == tower.Data.ID);

        if (_towers.Count >= MaxCount || (_towers.Count > 0 && _towers[0].Data.IsSpecial))
        {
            return false;
        }

        _towers.Add(tower);
        UpdateTowerPosition();
        return true;
    }

    public int GetTowerCount()
    {
        return _towers.Count;
    }

    public ITower GetTower()
    {
        return (_towers.Count > 0) ? _towers[0] : null;
    }

    public void RemoveTowerAll()
    {
        _towers.Clear();
    }

    private void UpdateTowerPosition()
    {
        if (_towers.Count == 1)
        {
            _towers[0].Transform.position = _transform.position;
        }
        else
        {
            Vector3[] offsets = _towers.Count switch
            {
                2 => Positions2,
                3 => Positions3,
                _ => null
            };

            if (offsets == null) return;

            for (int index = 0; index < offsets.Length; index++)
            {
                _towers[index].Transform.position = _transform.position + offsets[index];
            }
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (_towers.Count == 0) return;

        GridSelectionHandler.Select(this);

#if UNITY_EDITOR
        ITower tower = _towers[0];
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Tower : {tower.Data.TowerName}");
        sb.AppendLine($"Damage : {tower.Damage}");
        sb.AppendLine($"Range : {tower.Range}");
        sb.AppendLine($"FireRate : {tower.FireRate}");
        Debug.Log(sb.ToString());
#endif
    }

    public void OnSelect()
    {
        _towers[0].Selectable.OnSelect();
    }

    public void OnDeselect()
    {
        _towers[0].Selectable.OnDeselect();
    }
}
