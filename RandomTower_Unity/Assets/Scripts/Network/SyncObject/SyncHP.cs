public class SyncHP : BaseSync<SyncHP.Data>
{
    private BaseEnemy _enemy;

    public override string SyncType => "hp";
    
    private void Awake()
    {
        _enemy = GetComponent<BaseEnemy>();
    }


    protected override Data GetCurrentData()
    {
        return new Data { HP = _enemy.CurrentHP };
    }

    protected override void ApplyData(Data data)
    {
        float amount = (_enemy.CurrentHP - data.HP);
        _enemy.TakeDamage(amount);
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
