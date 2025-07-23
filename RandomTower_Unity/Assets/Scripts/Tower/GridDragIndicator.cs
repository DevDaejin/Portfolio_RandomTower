using UnityEngine;
using UnityEngine.UI;

public class GridDragIndicator : MonoBehaviour, IDrag
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    public void Initialize()
    {
        ActivePoints(false);
    }

    public void OnBeginDrag(Vector3 startPosition)
    {
        _startPoint.position = startPosition;
    }
    public void OnDrag(Vector3 position)
    {
        ActivePoints(true);

        if (position == _startPoint.position)
        {
            _endPoint.gameObject.SetActive(false);
            return;
        }

        _endPoint.position = position;
        _endPoint.rotation = Quaternion.LookRotation(_endPoint.position - _startPoint.position);
    }
    public void OnEndDrag(Vector3 endPosition)
    {
        ActivePoints(false);
    }

    private void ActivePoints(bool isAct)
    {
        _startPoint.gameObject.SetActive(isAct);
        _endPoint.gameObject.SetActive(isAct);
    }

}
