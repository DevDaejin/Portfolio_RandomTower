using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LoadingUI LoadingUI => _loadingUI ??= GetComponentInChildren<LoadingUI>(true);
    [SerializeField] private LoadingUI _loadingUI;

    WaitForSeconds waitting = new WaitForSeconds(0.5f);

    public string CurrentScene { get; private set; } = string.Empty;

    public void LoadSceneAsync(string name)
    {
        CurrentScene = name;
        Scene scene = SceneManager.GetSceneByName(CurrentScene);
        if (scene == null)
        {
            Debug.Log("Scene name is not valid");
            return;
        }

        LoadingUI.Show();
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        Debug.Log(CurrentScene);
        AsyncOperation operation = SceneManager.LoadSceneAsync(CurrentScene);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if(operation.progress >= 0.9f)
            {
                yield return waitting;
                operation.allowSceneActivation = true;
                LoadingUI.Hide();
            }

            yield return null;
        }
    }
}
