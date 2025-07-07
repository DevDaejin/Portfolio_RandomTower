using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _uiManagerPrefab;
    [SerializeField] private TowerDatabase _towerDB;

    private const string Main = "Main";
    private const string Lobby = "Lobby";
    private const string Game = "Game";
    private const int BasicGrade = 1;

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

    public Dispatcher Dispatcher => _dispatcher ??= GetComponent<Dispatcher>();
    private Dispatcher _dispatcher;

    public LocalDataManager Data
    {
        get
        {
            if (_dataManager == null)
            {
                _dataManager = GetDataManager();
            }
            return _dataManager;
        }
    }
    private LocalDataManager _dataManager;

    public ResourceManager Resource=> _rescoureManager ??= new ResourceManager();
    private ResourceManager _rescoureManager;

    public enum Scenes { Main, Lobby, Game };

    public TowerDatabase TowerDB => _towerDB;
    public Dictionary<string, TowerDataConfig> ActivedTowers { get; private set; } = new();

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

        UI.Global.OnNetworkConfirmClicked = () => LoadScene(Scenes.Main);
        UI.Global.OnNetworkWaittingClicked = () => LoadScene(Scenes.Lobby);

        LoadActivedTowers();

        var loadedGem = new KeyValuePair<ResourceManager.ResourceType, int>(ResourceManager.ResourceType.Gem, Data.Loaded.Gem);
        Resource.Initialize(loadedGem);
    }

    private void ActiveNetowrkPanel()
    {
        UI.Main.DeactiveConnectPanel();
        UI.Global.Set(GlobalUI.GlobalUIOption.Error);
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

        Scene.LoadSceneAsync(sceneName);
    }

    private LocalDataManager GetDataManager()
    {
        if (_dataManager == null)
        {
            List<string> towerNames = new();
            foreach (var tower in _towerDB.Towers)
            {
                if (tower.Data.Grade == BasicGrade
                    && !towerNames.Contains(tower.Data.TowerName))
                {
                    towerNames.Add(tower.Data.TowerName);
                }
            }
            return new LocalDataManager(towerNames);
        }
        return _dataManager;
    }

    private void LoadActivedTowers()
    {
        ActivedTowers.Clear();
        foreach (var towerName in Data.Loaded.GainedTowerNames)
        {
            ActivedTowers[towerName] = _towerDB.GetTowersByName(towerName);
        }
    }
}