public class InitializeData
{
    public readonly int MaxTower;
    public readonly int MaxEnemy;
    public readonly float WaveDuration;
    public readonly int InitialGoldAmount;

    public InitializeData(int maxTower, int maxEnemy, float waveDuration, int initialGoldAmount)
    {
        MaxTower = maxTower;
        MaxEnemy = maxEnemy;
        WaveDuration = waveDuration;
        InitialGoldAmount = initialGoldAmount;
    }
}
