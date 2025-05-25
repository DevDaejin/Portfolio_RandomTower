using UnityEngine;

public class IDGenerator
{
    private int _nextID = 1;
    public int Get() => _nextID++;
}
