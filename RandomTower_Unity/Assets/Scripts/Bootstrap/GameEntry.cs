using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private GameObject GameManagerPrefab;

    private void Awake()
    {
        GameManager gameManager = Instantiate(GameManagerPrefab).GetComponent<GameManager>();
        //gameManager.
    }
}
