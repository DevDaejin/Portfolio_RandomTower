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
        Debug.Log("a14i12u3490128374");
        var damage = _enemy.CurrentHP - data.Hp;
        _enemy.TakeDamage(damage);
        
    }

    protected override bool Equals(SyncHPData a, SyncHPData b)
    {
        Debug.Log("asfasdfsdf");
        if (a.Hp - b.Hp == 0) return true;
        return a.Hp == b.Hp;
    }
}
