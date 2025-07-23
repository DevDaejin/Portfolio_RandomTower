using System;

public class InGameUISetting
{
    public int MaxWave = 0;
    public int MaxEnemy = 0;
    public int MaxTower = 0;

    public float Time = 0;
    public int Gold = 0;

    public bool IsHost = false;

    public Action OnWave = null;
    public Action OnSpawnTower = null;
    public Action OnUpgrade = null;

    public Action OnMenu = null;
    public Action OnRetry = null;
    public Action OnGoToLobby = null;

    public Action OnMerge = null;
    public Action OnSell = null;

    public Func<int> OnSpawnPrice = null;
    public Func<int> OnUpgradePrice = null;
    public Func<int[]> OnUpgradeProbability = null;

    public Func<int> OnSuccessReward = null;
    public Func<int> OnFailedReward = null;
}
