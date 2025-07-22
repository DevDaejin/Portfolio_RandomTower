using System;

public class InGameUISetting
{
    public int MaxWave;
    public int MaxEnemy;
    public int MaxTower;

    public float Time;
    public int Gold;

    public bool IsHost;

    public Action OnWave;
    public Action OnSpawnTower;
    public Action OnUpgrade;

    public Action OnMenu;
    public Action OnRetry;
    public Action OnGoToLobby;

    public Action OnMerge;
    public Action OnSell;

    public Func<int> OnSpawnPrice;
    public Func<int> OnUpgradePrice;
    public Func<int[]> OnUpgradeProbability;

    public Func<int> OnSuccessReward;
    public Func<int> OnFailedReward;
}
