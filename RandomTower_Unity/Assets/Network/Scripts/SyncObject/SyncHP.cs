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

    protected override void FillData(SyncHPData target)
    {
        target.Hp = _enemy.CurrentHP;
    }

    protected override void ApplyData(SyncHPData data)
    {
        var damage = _enemy.CurrentHP - data.Hp;
        if (damage > 0)
        {
            _enemy.TakeDamage(damage);
            return;
        }
    }

    protected override bool Equals(SyncHPData a, SyncHPData b)
    {
        return a.Hp == b.Hp;
    }
}
 