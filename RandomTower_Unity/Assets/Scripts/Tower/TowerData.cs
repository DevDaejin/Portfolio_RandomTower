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

    public float Damage => _damage;
    [SerializeField] private float _damage;

    public float Range => _range;
    [SerializeField] private float _range;

    public float FireRate => _fireRate;
    [SerializeField] private float _fireRate;

    public int TargetCount => _targetCount;
    [SerializeField] private int _targetCount;

    public int GemCoast => _gemCoast;
    [SerializeField] private int _gemCoast;

    public int Level { get; set; } = 1;
    private int TowerMaxLevel = 5;
    public bool IsUpgradeable => Level < TowerMaxLevel;

    public bool IsSpecial => Grade == MaxGrade;
    private const int MaxGrade = 4;
}
