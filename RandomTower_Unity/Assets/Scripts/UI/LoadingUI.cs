using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _tipTextEvent;
    [SerializeField] private List<LocalizedString> _tipKeys = new();

    // temp
    [SerializeField] private Image _loadingImg;

    private bool _isShow;

    private bool _isUp;
    private float _fillAmountStart;
    private float _fillAmountEnd;
    private float _loadingAnimationTime;

    private const float LoadingAnimationSpeed = 2f;

    public void Show()
    {
        Initialize();
        ShowTips();
        gameObject.SetActive(true);
    }
      
    private void Initialize()
    {
        _isShow = true;
        _isUp = true;
        UpdateImageAnimCondition(_isUp);
    }

    private void Update()
    {
        LoadingAnimation();
    }

    private void ShowTips()
    {
        _tipTextEvent.StringReference = _tipKeys[Random.Range(0, _tipKeys.Count)];
    }

    private void LoadingAnimation()
    {
        if (!_isShow) return;

        if (_loadingAnimationTime > 1 || _loadingAnimationTime < 0)
        {
            _isUp = !_isUp;
            UpdateImageAnimCondition(_isUp);
        }

        _loadingAnimationTime += Time.deltaTime * LoadingAnimationSpeed;

        _loadingImg.fillAmount = Mathf.Lerp(_fillAmountStart, _fillAmountEnd, _loadingAnimationTime);
    }

    private void  UpdateImageAnimCondition(bool isUp)
    {
        _fillAmountStart = isUp ? 0 : 1;
        _fillAmountEnd = isUp ? 1 : 0;
        _loadingImg.transform.localScale = isUp ? Vector3.one : new Vector3(-1, 1, 1);
        _loadingAnimationTime = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
