using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public int ID => id;
    [SerializeField] private int id;

    public string EnemyName => enemyName;
    [SerializeField] private string enemyName;

    public GameObject Prefab => prefab;
    [SerializeField] private GameObject prefab;

    public float MaxHP => maxHP;
    [SerializeField] private float maxHP;

    public float Defence => defence;
    [SerializeField] private float defence;

    public float MoveSpeed => moveSpeed;
    [SerializeField] private float moveSpeed;

    public int RewardGold => rewardGold;
    [SerializeField] private int rewardGold;
}
