using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXHandler : MonoBehaviour
{
    Button _button;
    private void Start()
    {
        _button ??= GetComponent<Button>();
        _button.onClick.AddListener(() => GameManager.Instance.Sound.PlayBaiscButton());
    }
}
