using Sync;
using UnityEngine;

public class SyncHP : BaseSync<SyncHPData>
{
    private BaseEnemy _enemy;

    public override string SyncType => "hp";

    protected override void Awake()
    {
        base.Awake();
        _enemy = GetComponent<BaseEnemy>();
    }

    protected override SyncHPData GetCurrentData()
    {
        _currentData.Hp = _enemy.CurrentHP;
        return _currentData;
    }

    protected override void ApplyData(SyncHPData data)
    {
        _receivedData  = data;
        _enemy.CurrentHP = _receivedData.Hp;
    }

    protected override bool Equals(SyncHPData a, SyncHPData b)
    {
        return Near(a.Hp, b.Hp);
    }

    private bool Near(float a, float b, float epsilon = 0.001f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }
}
