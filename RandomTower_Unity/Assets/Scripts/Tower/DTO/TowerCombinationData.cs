using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerCombinationData", menuName = "Random TD/TowerCombinationData")]

public class TowerCombinationData : ScriptableObject
{
    public List<TowerDataConfig> RequiredTowers;
    public TowerDataConfig ResultTower;
}
