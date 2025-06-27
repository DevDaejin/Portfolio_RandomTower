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

    private TowerData _data;
    private bool _isLocked = false;

    public void Initialize(TowerData data, Action<TowerData> onButton, Action<TowerData> onLockButton = null)
    {
        _data = data;

        _name.text = _data.TowerName;
        _picture.sprite = _data.TowerSprite;

        _onButton = onButton;
        _button.onClick.AddListener(() => _onButton.Invoke(_data));

        _onLockButton = onLockButton;
        _isLocked = _onLockButton != null; 
        if (_isLocked)
        {
            _lockerButton.onClick.AddListener(() => _onLockButton.Invoke(_data));
        }

        _lockerButton.transform.gameObject.SetActive(_isLocked);
    }
}
 