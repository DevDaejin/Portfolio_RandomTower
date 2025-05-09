using System;
using UnityEngine;

[Serializable]
public class TowerData
{
    public int Grade => grade;
    [SerializeField] private int grade;

    public int ID => id;
    [SerializeField] private int id;

    public string TowerName => towerName;
    [SerializeField] private string towerName;

    public GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public float Damage => damage;
    [SerializeField] private float damage;

    public float Range => range;
    [SerializeField] private float range;

    public float FireRate => fireRate;
    [SerializeField] private float fireRate;

    public int TargetCount => targetCount;
    [SerializeField] private int targetCount;

    public bool IsSpecial => Grade == MaxGrade;
    private const int MaxGrade = 4;
}
