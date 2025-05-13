using UnityEngine;

public class TowerRangeViewer : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private GameObject _plane;

    private const float RotateSpeed = 20f;
    private const int LineSegment = 64;
    private const float PlaneHeight = 0.05f;

    private void Update()
    {
        if(_line.gameObject.activeInHierarchy && _plane.activeInHierarchy)
        {
            RotateLine();
        }
    }

    public void Active(float radius)
    {
        Show(true);

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        SetLine(radius);
        SetPlane(radius);
    }

    public void Deactive()
    {
        Show(false);
    }

    private void SetLine(float radius)
    {
        _line.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if(_line.positionCount != LineSegment + 1)
        {
            _line.positionCount = LineSegment + 1;
        }

        for (int index = 0; index <= LineSegment; index++)
        {
            float angle = index * Mathf.PI * 2 / LineSegment;
            Vector3 position =
                (Vector3.right * Mathf.Cos(angle) + Vector3.forward * Mathf.Sin(angle)) * radius;

            _line.SetPosition(index, position);
        }
    }

    private void RotateLine()
    {
        _line.transform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
    }

    private void SetPlane(float radius)
    {
        float diameter = 2 * radius;
        Vector3 scale = (Vector3.right + Vector3.forward) * diameter + (Vector3.up * PlaneHeight);
        _plane.transform.localScale = scale;
        _plane.transform.localPosition = Vector3.zero;
    }

    private void Show(bool isAct)
    {
        _line.gameObject.SetActive(isAct);
        _plane.SetActive(isAct);
    }
}
