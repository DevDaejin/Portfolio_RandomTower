using UnityEngine;

public class SyncHP : BaseSync<SyncHP.Data>
{
    private BaseEnemy _enemy;

    public override string SyncType => "hp";
    
    private void Awake()
    {
        _enemy = GetComponent<BaseEnemy>();
        _enemy.OnTakeDamage += OnTakeDamage;
    }

    private void OnDestroy()
    {
        if (_enemy != null)
        {
            _enemy.OnTakeDamage -= OnTakeDamage;
        }
    }

    private void OnTakeDamage(BaseEnemy enemy, float damage)
    {
        _syncedData = GetCurrentData();
    }

    protected override Data GetCurrentData()
    {
        return new Data { HP = _enemy.CurrentHP};
    }

    protected override void ApplyData(Data data)
    {
        _enemy.CurrentHP = data.HP;
    }

    protected override bool Equals(Data a, Data b)
    {
        return a.HP == b.HP;
    }

    public class Data
    {
        public float HP;
    }
}
