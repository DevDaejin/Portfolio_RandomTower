using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Button _lockerButton;
    [SerializeField] private Image _picture;
    [SerializeField] private TMP_Text _name;

    public Action<TowerData> _onButton;
    public Action<TowerData> _onLockButton;
    public TowerData Data { get; private set; }
    private bool _isLocked = false;

    public void Initialize(TowerData data, Action<TowerData> onButton, Action<TowerData> onLockButton = null)
    {
        Data = data;

        _name.text = Data.TowerName;
        _picture.sprite = Data.TowerSprite;

        _onButton = onButton;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onButton?.Invoke(Data));

        _onLockButton = onLockButton;
        _isLocked = _onLockButton != null;
        if (_isLocked)
        {
            _lockerButton.onClick.RemoveAllListeners();
            _lockerButton.onClick.AddListener(() => _onLockButton?.Invoke(Data));
        }

        _lockerButton.transform.gameObject.SetActive(_isLocked);
    }
}
