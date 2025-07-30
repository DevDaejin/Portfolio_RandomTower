using UnityEngine;

public class GameEntry : MonoBehaviour
{
    [SerializeField] private GameObject GameManagerPrefab;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instantiate(GameManagerPrefab).GetComponent<GameManager>();
        GameManager.Instance.Initialize();
    }

    private void Start()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Main);
    }
}
