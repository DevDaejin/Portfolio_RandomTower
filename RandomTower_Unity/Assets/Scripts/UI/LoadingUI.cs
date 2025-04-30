using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _tipTextEvent;
    [SerializeField] private List<LocalizedString> _tipKeys = new();

    // temp
    [SerializeField] private Image _loadingImg;
    
    private bool _isUp = true;
    private float _fillAmountStart = 0;
    private float _fillAmountEnd = 1;
    private float _loadingAnimationTime = 0;
    
    private const float LoadingAnimationSpeed = 1f;

    private void OnEnable()
    {
        ShowTips();
        _loadingImg.fillAmount = 0;
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
        _loadingAnimationTime += Time.deltaTime * LoadingAnimationSpeed;

        if(_loadingAnimationTime > 1 || _loadingAnimationTime < 0)
        {
            _isUp = !_isUp;
            _fillAmountStart = _isUp ? 0 : 1;
            _fillAmountEnd = _isUp ? 1 : 0;
            _loadingImg.transform.localScale = _isUp ? Vector3.one : new Vector3(-1, 1, 1);
            _loadingAnimationTime = 0;            
        }

        _loadingImg.fillAmount = Mathf.Lerp(_fillAmountStart, _fillAmountEnd, _loadingAnimationTime);
    }
}
