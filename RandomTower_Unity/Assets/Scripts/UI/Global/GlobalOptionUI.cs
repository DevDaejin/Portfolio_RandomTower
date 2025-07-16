using UnityEngine;

public class GlobalOptionUI : MonoBehaviour
{
    [SerializeField] private GameObject _optionPanel;

    public void ActiveUI(bool isAct) => _optionPanel.SetActive(isAct);
}
