using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LoadingUI LoadingUI => _loadingUI ??= GetComponentInChildren<LoadingUI>(true);
    [SerializeField] private LoadingUI _loadingUI;
    private bool _isLoading;

    public string CurrentSceneName { get; private set; } = string.Empty;

    public void LoadSceneAsync(string name)
    {
        if(_isLoading)
        {
            Debug.Log("duplicated load blocked");
            return;
        }
        
        if (!Application.CanStreamedLevelBeLoaded(name))
        {
            Debug.Log($"Invalid scene : {name}");
            return;
        }

        _isLoading = true;
        CurrentSceneName = name;
        LoadingUI.Show();
        StartCoroutine(LoadSceneRoutine());
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
