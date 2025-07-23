using System;
using UnityEngine;

public class InputController
{
    public Action<Vector3, Vector3> OnDragEnd;

    private Vector3 _startPosition;

    private Vector3 _startWorldPosition;
    private Vector3 _endWorldPosition;

    private bool _isDragging;
    private float _dragThreshold = 10f;

    private IDrag _dragTarget;
    private ISelect _selectTarget;

    private Camera _cam;
    private const string InteractableLayer = "Interactable";


    public void Raycast()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _startPosition = Input.mousePosition;
            _isDragging = false;

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            if (Raycast(_startPosition, out var hit))
            {
                _dragTarget = hit.collider.GetComponent<IDrag>();

                if (_selectTarget != null)
                {
                    _selectTarget?.OnDeselect();
                    _selectTarget = null;
                }
                _selectTarget = hit.collider.GetComponent<ISelect>();
            }
        }

        if(Input.GetMouseButton(0))
        {
            var sqrDistacne = (_startPosition - Input.mousePosition).sqrMagnitude;
            if(IsDragable(sqrDistacne))
            {
                _isDragging = true;
                if (Raycast(_startPosition, out var hit))
                {
                    _startWorldPosition = hit.transform.position;
                    _dragTarget?.OnBeginDrag(_startWorldPosition);
                }
            }

            if(_isDragging)
            {
                if (Raycast(Input.mousePosition, out var hit))
                {
                    _dragTarget?.OnDrag(hit.transform.position);
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(_isDragging)
            {
                if (Raycast(Input.mousePosition, out var hit))
                {
                    _endWorldPosition = hit.transform.position;
                    _dragTarget?.OnEndDrag(_endWorldPosition);
                    OnDragEnd?.Invoke(_startWorldPosition, _endWorldPosition);
                }
            }
            else
            {
                if(Raycast(Input.mousePosition, out var hit))
                {
                    _selectTarget?.OnSelect();
                }
                else
                {
                    _selectTarget?.OnDeselect();
                }
            }

            _dragTarget = null;
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

    private bool IsDragable(float sqrDistance) => (!_isDragging && sqrDistance > (_dragThreshold * _dragThreshold) && _dragTarget != null);
}
