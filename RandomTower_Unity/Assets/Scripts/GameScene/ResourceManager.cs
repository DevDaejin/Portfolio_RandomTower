using System;
using UnityEngine;

public class ResourceManager
{
    public int Gold { get; private set; }

    public void Initialize()
    {
        Gold = 0;
    }

    public void EarnGold(int amount)
    {
        Gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if(Gold - amount >= 0)
        {
            Gold -= amount;
            return true;
        }

        return false;
    }
}
