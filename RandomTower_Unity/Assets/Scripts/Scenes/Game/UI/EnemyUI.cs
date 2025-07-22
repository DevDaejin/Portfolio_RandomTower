using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    public void Initialize()
    {
        _hpSlider.value = 1;
        RectTransform fillRect = _hpSlider.fillRect;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
    }

    public void UpdateHP(float ratio)
    {
        _hpSlider.value = ratio;
    }
}
