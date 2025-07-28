using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class SceneLoader : MonoBehaviour
{
    public LoadingUI LoadingUI => _loadingUI ??= GetComponentInChildren<LoadingUI>(true);
    [SerializeField] private LoadingUI _loadingUI;
    private bool _isLoading;

    public string CurrentSceneName { get; private set; } = string.Empty;
    public Scenes CurrentScenes => NameToScenes(CurrentSceneName);

    public void LoadSceneAsync(Scenes scene)
    {
        CurrentSceneName = ScenesToName(scene);

        if (_isLoading)
        {
            Debug.Log("duplicated load blocked");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(CurrentSceneName))
        {
            Debug.Log($"Invalid scene : {name}");
            return;
        }

        _isLoading = true;
        LoadingUI.Show();
        StartCoroutine(LoadSceneRoutine());
    }

    private string ScenesToName(Scenes scene)
    {
        return scene switch
        {
            Scenes.Main => "Main",
            Scenes.Game => "Game",
            Scenes.Lobby => "Lobby",
            _ => string.Empty,
        };
    }

    private Scenes NameToScenes(string name)
    {
        return name switch
        {
            "Main" => Scenes.Main,
            "Game" => Scenes.Game,
            "Lobby" => Scenes.Lobby,
            _ => Scenes.Main,
        };
    }

    private IEnumerator LoadSceneRoutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(CurrentSceneName);
        operation.allowSceneActivation = false;


        while (operation.progress < 0.9f) yield return null;

        yield return null;
        operation.allowSceneActivation = true;
        yield return null;

        LoadingUI.Hide();
        _isLoading = false;
    }
}
