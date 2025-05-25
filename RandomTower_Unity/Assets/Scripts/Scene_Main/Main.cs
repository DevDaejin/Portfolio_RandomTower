using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.UI.Initialize(UIManager.UIType.Main);
    }
}
