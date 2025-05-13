using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar
{
    private Slider _hpBarSlider;
    private float _maxHP;
    private float _currentHP;

    public HealthBar(Slider healthBarSldiir, float maxHP)
    {
        _hpBarSlider = healthBarSldiir;
        _maxHP = maxHP;
        _currentHP = _maxHP;
    }

    public void OnUpdateHP(float amount)
    {
        _currentHP -= amount;

        if (_currentHP < 0)
            _currentHP = 0;

        _hpBarSlider.value = Ratio();
    }

    private float Ratio()
    {
        return Mathf.Clamp01(_currentHP / _maxHP);
    }
}
