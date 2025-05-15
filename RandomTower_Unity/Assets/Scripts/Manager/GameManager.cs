using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _uiManagerPrefab;

    public static GameManager Instance { get; private set; }

    public UIManager UIManager
    {
        get
        {
            if(_uiManager == null)
            {
                _uiManager = Instantiate(_uiManagerPrefab).GetComponent<UIManager>();
            }

            return _uiManager;
        }

    }
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

    public void Initialize()
    {
        //UIManager.Initialize();
    }

    private void Start()
    {
        //LoadScene(MainScene);
    }

    public void LoadScene(string name) => SceneLoader.LoadSceneAsync(name);
}