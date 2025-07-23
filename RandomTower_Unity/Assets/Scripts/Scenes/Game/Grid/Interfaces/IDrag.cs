using UnityEngine;

public interface IDrag
{
    void OnBeginDrag(Vector3 startPosition);
    void OnDrag(Vector3 currentPosition);
    void OnEndDrag(Vector3 endPosition);
}
