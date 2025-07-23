using UnityEngine;

public class InputController
{
    private Vector3 _startPosition;
    private bool _isDragging;
    private float _dragThreshold = 5f;

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

            if(Raycast(_startPosition, out var hit))
            {
                _dragTarget = hit.collider.GetComponent<IDrag>();

                if(_selectTarget != null)
                {
                    _selectTarget.OnDeselect();
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
                _dragTarget?.OnBeginDrag(_startPosition);
            }

            if(_isDragging)
            {
                _dragTarget?.OnDrag(Input.mousePosition);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(_isDragging)
            {
                _dragTarget?.OnEndDrag(Input.mousePosition);
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
        Ray ray = _cam.ScreenPointToRay(_startPosition);

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject.layer == LayerMask.NameToLayer(InteractableLayer);
        }

        return false;
    }

    private bool IsDragable(float sqrDistance)
    {
        return (!_isDragging && sqrDistance > (_dragThreshold * _dragThreshold) && _dragTarget != null);
    }
}
