using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string EnemyName;
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
