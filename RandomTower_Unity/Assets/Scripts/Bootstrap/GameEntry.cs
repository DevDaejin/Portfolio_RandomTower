using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private GameObject GameManagerPrefab;

    private void Awake()
    {
        Instantiate(GameManagerPrefab).GetComponent<GameManager>();
        GameManager.Instance.Initialize();
    }
}
