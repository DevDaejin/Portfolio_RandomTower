using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    public void Initialize()
    {
        _hpSlider.value = 1;
    }

    public void UpdateHP(float ratio)
    {
        _hpSlider.value = ratio;
    }
}
