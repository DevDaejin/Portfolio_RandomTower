// InputController.cs

using System;
using UnityEngine;

public class InputController
{
    public Action<ISelect> OnSelect;
    public Action OnDeselect;
    public Action<Vector3, Vector3> OnDragEnd;

    private Vector3 _startPosition;
    private Vector3 _startWorldPosition;
    private Vector3 _endWorldPosition;

    private bool _isDragging;
    private float _dragThreshold = 10f;

    private IDrag _dragTarget;
    public ISelect SelectTarget => _selectTarget;
    private ISelect _selectTarget;

    private Camera _cam;
    private const string InteractableLayer = "Interactable";

    public void Raycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        if (Input.GetMouseButton(0))
        {
            Drag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_isDragging)
            {
                EndDrag();
            }
            else
            {
                Select();
            }

            _dragTarget = null;
        }
    }

    private void StartDrag()
    {
        _startPosition = Input.mousePosition;
        _isDragging = false;

        if (Raycast(_startPosition, out var hit))
        {
            _dragTarget = hit.collider.GetComponent<IDrag>();
        }
    }

    private void Drag()
    {
        var sqrDistance = (_startPosition - Input.mousePosition).sqrMagnitude;

        if (IsDragable(sqrDistance))
        {
            _isDragging = true;

            if (Raycast(_startPosition, out var hit))
            {
                _startWorldPosition = hit.transform.position;
                _dragTarget?.OnBeginDrag(_startWorldPosition);
            }
        }

        if (_isDragging)
        {
            if (Raycast(Input.mousePosition, out var hit))
            {
                _dragTarget?.OnDrag(hit.transform.position);
            }
        }
    }

    private void EndDrag()
    {
        SelectTarget?.OnDeselect();
        OnDeselect?.Invoke();

        if (Raycast(Input.mousePosition, out var hit))
        {
            _endWorldPosition = hit.transform.position;
            _dragTarget?.OnEndDrag(_endWorldPosition);

            if (_dragTarget is TowerGrid grid)
            {
                if (grid.GetTowerCount() != 0)
                {
                    OnDragEnd?.Invoke(_startWorldPosition, _endWorldPosition);
                }
            }
        }
        else
        {
            _dragTarget?.OnEndDrag(_startWorldPosition);
        }
    }

    private void Select()
    {
        if (Raycast(Input.mousePosition, out var hit))
        {
            var newSelect = hit.collider.GetComponent<ISelect>();

            if (_selectTarget != newSelect)
            {
                _selectTarget?.OnDeselect();
                OnDeselect?.Invoke();
            }

            _selectTarget = newSelect;
            _selectTarget?.OnSelect();
            OnSelect?.Invoke(newSelect);
        }
        else
        {
            _selectTarget?.OnDeselect();
            OnDeselect?.Invoke();
            _selectTarget = null;
        }
    }

    private bool Raycast(Vector3 screenPosition, out RaycastHit hit)
    {
        _cam ??= Camera.main;
        Ray ray = _cam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject.layer == LayerMask.NameToLayer(InteractableLayer);
        }

        return false;
    }

    private bool IsDragable(float sqrDistance)
    {
        return !_isDragging && sqrDistance > (_dragThreshold * _dragThreshold) && _dragTarget != null;
    }


}
