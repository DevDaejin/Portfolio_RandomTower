using UnityEngine;

public class PlayerPrefabReset : MonoBehaviour
{
    [ContextMenu("[Player prefs] 모두 초기화")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("모두 초기화");
    }

    [ContextMenu("[Player prefs] 게임 저장 데이터 초기화")]
    public void ClearGameSaveOnly()
    {
        PlayerPrefs.DeleteKey("game");
        PlayerPrefs.Save();

        Debug.Log("게임 저장 데이터 초기화");
    }

    [ContextMenu("[Player prefs] 옵션 저장 데이터 초기화")]
    public void ClearOptionOnly()
    {
        PlayerPrefs.DeleteKey("option");
        PlayerPrefs.Save();

        Debug.Log("옵션 데이터 초기화");
    }
}
