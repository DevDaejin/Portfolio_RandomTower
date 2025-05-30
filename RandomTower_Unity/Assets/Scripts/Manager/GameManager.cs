using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _uiManagerPrefab;

    public static GameManager Instance { get; private set; }

    private const string Main = "Main";
    private const string Lobby = "Lobby";
    private const string Game = "Game";

    public UIManager UI
    {
        get
        {
            if (_uiManager == null)
            {
                _uiManager = Instantiate(_uiManagerPrefab).GetComponent<UIManager>();
                _uiManager.transform.SetParent(transform);
            }

            return _uiManager;
        }

    }
    private UIManager _uiManager;

    public NetworkManager Network => _networkManager ??= GetComponent<NetworkManager>();
    private NetworkManager _networkManager;

    public SceneLoader SceneLoader => _sceneLoader ??= GetComponentInChildren<SceneLoader>();
    private SceneLoader _sceneLoader;

    public Dispatcher Dispatcher => _dispatcher ??= GetComponent<Dispatcher>();
    private Dispatcher _dispatcher;


    public enum Scenes { Main, Lobby, Game };

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
        UI.Initialize(UIManager.UIType.None);
    }

    public void LoadScene(Scenes scene)
    {
        string sceneName = string.Empty;

        switch (scene)
        {
            case Scenes.Main:
                sceneName = Main;
                break;
            case Scenes.Lobby:
                sceneName = Lobby;
                break;
            case Scenes.Game:
                sceneName = Game;
                break;
        }

        SceneLoader.LoadSceneAsync(sceneName);
    }
}