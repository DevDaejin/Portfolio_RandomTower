using UnityEngine;

public interface IProjectileMovement
{
    void Move(Transform transform, BaseEnemy target, float speed);
}

public interface IProjectileHitCheck
{
    bool HasHit(Vector3 projectilePosition, BaseEnemy target);
}