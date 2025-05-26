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
        UI.Main.OnMutliConfirm += OnMutliConfirm;
    }

    private void OnMutliConfirm(string ip, string port)
    {
        Network.Connect(ip, port);
    }

    //TODO: Delete it, ater testing
    public void TestConnect()
    {
        Network.Connect("127.0.0.1", "8765");
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