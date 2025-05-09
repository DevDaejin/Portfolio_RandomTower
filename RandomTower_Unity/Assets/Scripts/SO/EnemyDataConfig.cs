using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Random TD/EnemyData")]
public class EnemyDataConfig : ScriptableObject
{
    [SerializeField] private EnemyData _data;
    public EnemyData Data => _data;
}
