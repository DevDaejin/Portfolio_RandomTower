using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _enemyUIPrefab;
    [SerializeField] private GameObject _damageUIPrefab;

    private Pool<EnemyUI> _enemyUIPool;
    private Pool<DamageUI> _damageUIPool;
    private Camera _cam;

    private readonly Dictionary<BaseEnemy, EnemyUI> _uiDict = new();
    private const float HeightOffset = 0.5f;

    private void Awake()
    {
        _cam = Camera.main;
        Transform canvasTransform = _canvas.transform;
        _enemyUIPool = new Pool<EnemyUI>(_enemyUIPrefab, canvasTransform);
        _damageUIPool = new Pool<DamageUI>(_damageUIPrefab, canvasTransform);
    }

    private void LateUpdate()
    {
        foreach (KeyValuePair<BaseEnemy, EnemyUI> pair in _uiDict)
        {
            BaseEnemy enemy = pair.Key;
            if (!enemy.gameObject.activeInHierarchy) continue;
            pair.Value.transform.position = GetScreenPosition(enemy);
        }
    }

    public void Register(BaseEnemy enemy)
    {
        EnemyUI ui = _enemyUIPool.Get();
        ui.Initialize();

        if (!_uiDict.ContainsKey(enemy))
        {
            _uiDict.Add(enemy, ui);
        }

        enemy.OnTakeDamage += OnEnemyDamaged;
    }

    public void Unregister(BaseEnemy enemy)
    {
        if (_uiDict.TryGetValue(enemy, out EnemyUI ui))
        {
            _enemyUIPool.Return(ui);

            if (_uiDict.ContainsKey(enemy))
            {
                _uiDict.Remove(enemy);
            }
        }

        enemy.OnTakeDamage -= OnEnemyDamaged;
    }

    private void OnEnemyDamaged(BaseEnemy enemy, float amount)
    {
        if (_uiDict.TryGetValue(enemy, out EnemyUI ui))
        {
            ui.UpdateHP(enemy.GetHPRatio());
        }

        DamageUI damageUI = _damageUIPool.Get();
        damageUI.OnReturn = ReturnDamageUI;
        damageUI.Show(amount, GetScreenPosition(enemy));
    }

    private void ReturnDamageUI(DamageUI damageUI)
    {
        _damageUIPool.Return(damageUI);
    }

    private Vector2 GetScreenPosition(BaseEnemy enemy)
    {
        Vector3 world = enemy.transform.position + Vector3.up * HeightOffset;
        return _cam.WorldToScreenPoint(world);
    }

    public void ReturnAll()
    {
        _enemyUIPool.ReturnAll();
        _damageUIPool.ReturnAll();
    }
}
