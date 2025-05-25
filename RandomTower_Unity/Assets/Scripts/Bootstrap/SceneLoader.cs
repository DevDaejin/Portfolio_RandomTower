using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public LoadingUI LoadingUI => _loadingUI ??= GetComponentInChildren<LoadingUI>(true);
    [SerializeField] private LoadingUI _loadingUI;
    private float _loadDelay;

    public void LoadSceneAsync(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        if (scene == null)
        {
            Debug.Log("Scene name is not valid");
            return;
        }

        LoadingUI.Show();
        StartCoroutine(LoadSceneRoutine(name));
    }

    private IEnumerator LoadSceneRoutine(string name)
    {
        Debug.Log(name);
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            if(operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(_loadDelay);
                operation.allowSceneActivation = true;
                LoadingUI.Hide();
            }

            yield return null;
        }
    }
}
