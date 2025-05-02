using UnityEngine;
using UnityEngine.Tilemaps;

public class GridFactory : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private Vector2 _totalMapSize = new Vector2(20, 20);
    [SerializeField] private Vector2 _playableMapSize = new Vector2(8, 9);
    private int[] _pathRow = { 0, 4, 8 };
    private int[] _pathColumn = { 0, 7 };

    public void CreateGrid()
    {
        DeleteOldTiles();
        CreateTotalMap();
    }

    private void DeleteOldTiles()
    {
        Transform[] transformTree = GetComponentsInChildren<Transform>();

        if (transformTree.Length == 1) return;

        foreach (var branch in transformTree)
        {
            if (branch != transform)
            {
                DestroyImmediate(branch.gameObject);
            }
        }
    }

    private void CreateTotalMap()
    {
        Quaternion rotation = _tile.transform.rotation;
        Vector3 tileSize = _tile.transform.localScale;
        Vector3 position = Vector3.zero;

        Vector2Int totalSize = Vector2Int.RoundToInt(_totalMapSize);
        Vector2Int playableSize = Vector2Int.RoundToInt(_playableMapSize);

        // 1. 플레이어블 시작 위치 (전체 맵 기준)
        Vector2Int playableStart = new Vector2Int(
            Mathf.FloorToInt((totalSize.x - playableSize.x) * 0.5f),
            Mathf.FloorToInt((totalSize.y - playableSize.y) * 0.5f)
        );

        for (int row = 0; row < totalSize.y; row++)
        {
            for (int col = 0; col < totalSize.x; col++)
            {
                position.x = col * tileSize.x;
                position.z = row * tileSize.z;

                // 기본 높이 = 전체 타일 (비플레이어블)
                position.y = 1f;

                // 2. 플레이어블 영역 여부 확인
                if (col >= playableStart.x && col < playableStart.x + playableSize.x &&
                    row >= playableStart.y && row < playableStart.y + playableSize.y)
                {
                    // 플레이어블 내 상대 좌표
                    int playableCol = col - playableStart.x;
                    int playableRow = row - playableStart.y;

                    // 3. 경로에 해당하면 0.5, 아니면 0
                    if (System.Array.Exists(_pathRow, r => r == playableRow) ||
                        System.Array.Exists(_pathColumn, c => c == playableCol))
                    {
                        position.y = 0.5f;
                    }
                    else
                    {
                        position.y = 0f;
                    }
                }

                GetCalculatedPosition(ref position, _totalMapSize, tileSize);
                Instantiate(_tile, position, rotation, transform);
            }
        }
    }


    private void GetCalculatedPosition(ref Vector3 position, Vector2 gridSize, Vector3 tileSize)
    {
        position.x -= ((gridSize.x) * 0.5f) * tileSize.x - (tileSize.x * 0.5f);
        position.z -= ((gridSize.y) * 0.5f) * tileSize.z - (tileSize.z * 0.5f);
    }

}
