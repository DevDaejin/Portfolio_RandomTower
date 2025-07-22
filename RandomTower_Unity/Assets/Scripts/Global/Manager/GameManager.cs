using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _uiManagerPrefab;
    [SerializeField] private TowerDatabase _towerDB;

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

    public SceneLoader Scene => _sceneLoader ??= GetComponentInChildren<SceneLoader>();
    private SceneLoader _sceneLoader;

    public OptionManager Option => _optionManager ??= GetComponent<OptionManager>();
    private OptionManager _optionManager;

    public LocalDataManager Data => _dataManager ??= GetDataManager();
    private LocalDataManager _dataManager;

    public ResourceManager Resource=> _rescoureManager ??= new ResourceManager();
    private ResourceManager _rescoureManager;

    public enum Scenes { Main, Lobby, Game };

    public TowerDatabase TowerDB => _towerDB;

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
        Network.OnConnectFailed += ActiveNetowrkPanel;
        Network.OnError += ActiveNetowrkPanel;
        Network.OnClose += ActiveNetowrkPanel;

        LoadActivedTowers();

        var loadedGem = new KeyValuePair<ResourceManager.ResourceType, int>(ResourceManager.ResourceType.Gem, Data.Loaded.Gem);
        Resource.Initialize(loadedGem);
    }

    private void ActiveNetowrkPanel()
    {
        UI.Main.DeactiveConnectPanel();
        UI.Global.ShowNetworkError();
    }

    public void Initialize()
    {
        UI.Initialize(UIManager.UIType.None);
        Option.Initialize();
    }

    public void LoadScene(Scenes scene)
    {
        string sceneName = scene switch
        {
            Scenes.Main => Main,
            Scenes.Game => Game,
            Scenes.Lobby => Lobby,
            _ => string.Empty,
        };

        Scene.LoadSceneAsync(sceneName);
    }

    private LocalDataManager GetDataManager()
    {
        if (_dataManager == null)
        {
            return new LocalDataManager(TowerDB);
        }

        return _dataManager;
    }

    private void LoadActivedTowers()
    {
        _towerDB.ActiveTowers.Clear();
        foreach (var towerID in Data.Loaded.GainedTowerID)
        {
            _towerDB.ActiveTowers.Add(_towerDB.GetTowerByID(towerID));
        }
    }
}