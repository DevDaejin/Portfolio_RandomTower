using NUnit.Framework;
using System;
using UnityEngine;

[Serializable]
public class TowerData
{
    public int Grade => _grade;
    [SerializeField] private int _grade;

    public int ID => _id;
    [SerializeField] private int _id;

    public string TowerName => _towerName;
    [SerializeField] private string _towerName;

    public GameObject TowerPrefab => _towerPrefab;
    [SerializeField] private GameObject _towerPrefab;

    public Sprite TowerSprite => _towerSprite;
    [SerializeField] private Sprite _towerSprite;

    public GameObject ProjectilePrefab => _projectilePrefab;
    [SerializeField] private GameObject _projectilePrefab;

    public float ProjectileSpeed => _projectileSpeed;
    [SerializeField] private float _projectileSpeed;

    public float Damage => _damage * (1 + (0.1f * (Level - 1)));
    [SerializeField] private float _damage;

    public float Range => _range * (1 + (0.1f * (Level - 1)));
    [SerializeField] private float _range;

    public float FireRate => _fireRate * (1 + (0.1f * (Level - 1)));
    [SerializeField] private float _fireRate;

    public int TargetCount => _targetCount;
    [SerializeField] private int _targetCount;

    public int SpawnCoast => _spawnCoast;
    [SerializeField] private int _spawnCoast;

    public int BuyingCoast => _buyingCoast;
    [SerializeField] private int _buyingCoast;

    public int UpgradeCost => _upgradeCost * Level;
    [SerializeField] private int _upgradeCost;

    public int Level { get; set; } = 1;

    public bool IsUnique => Grade == MaxGrade;
    
    public bool LevelUp()
    {
        if (Level < MaxLevel)
        {
            Level++;
            return true;
        }

        return false;
    }

    private int MaxLevel = 5;
    public bool IsUpgradeable => Level < MaxLevel;
    private const int MaxGrade = 4;
}
