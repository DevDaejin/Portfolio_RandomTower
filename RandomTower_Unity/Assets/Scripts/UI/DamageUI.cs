using System;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private RectTransform _rectTransform;
    private bool _isAnimation;
    private float _duriation = 0.5f;
    private float _colorSpeed = 2;
    private float _elapsed;
    private Color _origin;
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

    public void Show(float damage, Vector3 position, Action<DamageUI> returnCallback)
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
        float time = Mathf.Clamp01(_elapsed / _duriation);

        if (time != 1)
        {
            _text.color = Color.Lerp(_text.color, _origin, time * _colorSpeed);
        }
        else
        {
            _isAnimation = false;
        }
    }
}
