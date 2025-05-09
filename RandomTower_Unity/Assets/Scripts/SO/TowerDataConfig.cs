using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Random TD/TowerData")]
public class TowerDataConfig : ScriptableObject
{
    [SerializeField] private TowerData _data;
    public TowerData Data => _data;
}
