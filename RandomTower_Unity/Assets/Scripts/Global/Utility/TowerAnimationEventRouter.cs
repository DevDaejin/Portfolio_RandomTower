using System;
using UnityEngine;

public class TowerAnimationEventRouter : MonoBehaviour
{
    public Action AttackCallback;

    public void Attack()
    {
        AttackCallback?.Invoke();
    }
}
