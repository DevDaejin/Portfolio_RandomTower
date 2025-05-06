using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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

    private float[] GetNormalizeWeights(int level)
    {
        float t = Mathf.InverseLerp(1, 5, level);
        float[] grades = new float[3];
        float total = 0;
        
        for (int index = 0; index < grades.Length; index++)
        {
            grades[index] = Mathf.Lerp(_gradeWeight[index].MinWeight, _gradeWeight[index].MaxWeight, t);
            total += grades[index];
        }

        for (int index = 0; index < grades.Length; index++)
        {
            grades[index] /= total;
        }

        return grades;
    }

    //public int GetRandomGrade(int level)
    //{ 
    //    Vector3 weights = GetNormalizeWeights(level);
    //}
}
