using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TowerChanceTable
{
    private readonly Dictionary<int, int[]> _chances = new();
    public int HighestLevel => _chances.Count;
    public TowerChanceTable()
    {
        _chances.Add(1, new[] { 100,    0,      0 });
        _chances.Add(2, new[] { 80,     20,     0 });
        _chances.Add(3, new[] { 70,     30,     0 });
        _chances.Add(4, new[] { 65,     30,     5 });
        _chances.Add(5, new[] { 60,     30,    10 });
    }

    public int[] GetProbability(int level)
    {
        _chances.TryGetValue(level, out var probability);
        return probability;
    }

    public int GetRandomGrade(int level)
    {
        if (_chances.TryGetValue(level, out var weight))
        {
            return GetByChance(weight);
        }

        return 0;
    }

    private int GetByChance(int[] weight)
    {
        float random = Random.value * 100;
        float sum = 0f;

        for (int index = 0; index < weight.Length; index++)
        {
            sum += weight[index];
            if(random <= sum)
            {
                return index + 1;
            }
        }

        return 0;
    }
}
