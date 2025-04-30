using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager UIManager => _uiManager ??= GetComponentInChildren<UIManager>();
    private UIManager _uiManager;

    public SceneLoader SceneLoader => _sceneLoader ??= GetComponentInChildren<SceneLoader>();
    private SceneLoader _sceneLoader;

    private const string MainScene = "Main";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadScene(MainScene);
    }

    public void LoadScene(string name) => SceneLoader.LoadSceneAsync(name);
}