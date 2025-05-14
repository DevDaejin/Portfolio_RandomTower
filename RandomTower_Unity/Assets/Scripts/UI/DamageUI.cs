using System;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private RectTransform _rectTransform;
    private bool _isAnimation;
    private float _duriation = 1f;
    private float _colorSpeed = 2;
    private float _elapsed;
    private Color _origin;

    public  Action<DamageUI> OnReturn;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _origin = _text.color;
    }

    private void Update()
    {
        if (!_isAnimation) return;
        AppearAnimation();
    }

    public void Show(float damage, Vector3 position)
    {
        Initialize();

        _text.text = Mathf.FloorToInt(damage).ToString();
        _rectTransform.position = position;
    }

    private void Initialize()
    {
        _isAnimation = true;
        _elapsed = 0;
        _text.color = Color.clear;
    }

    private void AppearAnimation()
    {
        _elapsed += Time.deltaTime;
        float timeRatio = Mathf.Clamp01(_elapsed / _duriation);

        if (timeRatio != 1)
        {
            _text.color = Color.Lerp(_text.color, _origin, timeRatio * _colorSpeed);
        }
        else
        {
            _isAnimation = false;
            OnReturn?.Invoke(this);
        }
    }
}
