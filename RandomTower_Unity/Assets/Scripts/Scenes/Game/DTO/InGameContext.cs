public class InGameContext
{
    public readonly TowerManager Tower;
    public readonly EnemyManager Enemy;
    public readonly WaveController Wave;
    public readonly ResourceManager Resource;
    public readonly NetworkManager Network;
    public readonly InGameUI UI;
    public readonly GlobalUI GlobalUI;
    public IDGenerator IDGenerator => _idGenerator ??= new(Network.ClientID);
    private IDGenerator _idGenerator;

    public InGameContext(TowerManager tower, EnemyManager enemy, WaveController wave)
    {
        Tower = tower;
        Enemy = enemy;
        Wave = wave;

        Resource = GameManager.Instance.Resource;
        Network = GameManager.Instance.Network;
        UI = GameManager.Instance.UI.InGame;
        GlobalUI = GameManager.Instance.UI.Global;
    }
}
