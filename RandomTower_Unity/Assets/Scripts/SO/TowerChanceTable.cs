using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "TowerChanceTable", menuName = "Random TD/TowerChanceTable")]
public class TowerChanceTable : ScriptableObject
{
    [Serializable]
    public class GradeWeight
    {
        [Range(1, 3)] public int Grade;
        [Range(0, 100)] public float MinWeight;
        [Range(0, 100)] public float MaxWeight;
    }

    [SerializeField] private GradeWeight[] _gradeWeight;

    private float[] GetNormalizedWeights(int level)
    {
        float t = Mathf.InverseLerp(1, 5, level);

        float grade1 = Mathf.Lerp(_gradeWeight[0].MinWeight, _gradeWeight[0].MaxWeight, t);
        float grade2 = Mathf.Lerp(_gradeWeight[1].MinWeight, _gradeWeight[1].MaxWeight, t);
        float grade3 = Mathf.Lerp(_gradeWeight[2].MinWeight, _gradeWeight[2].MaxWeight, t);

        float total = grade1 + grade2 + grade3;
        return new[] { grade1 / total, grade2 / total, grade3 / total };
    }

    public int GetRandomGrade(int level)
    {
        float[] weights = GetNormalizedWeights(level);
        float rand = Random.value;

        if (rand < weights[0]) return 1;
        if (rand < weights[0] + weights[1]) return 2;
        if (rand < weights[0] + weights[1] + weights[2]) return 3;
        
        return -1;
    }
}
