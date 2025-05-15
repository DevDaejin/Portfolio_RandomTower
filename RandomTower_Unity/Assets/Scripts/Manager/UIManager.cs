using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public MainUI Main => _mainUI;
    [SerializeField] private MainUI _mainUI;

    public InGameUI InGame => _inGameUI;
    [SerializeField] private InGameUI _inGameUI;

    public void Initialize(Type type)
    {
        if(type == typeof(MainUI))
        {
            _mainUI.gameObject.SetActive(true);
            _inGameUI.gameObject.SetActive(false);
        }
        if (type == typeof(InGameUI))
        {
            _mainUI.gameObject.SetActive(false);
            _inGameUI.gameObject.SetActive(true);
        }
    }
}
