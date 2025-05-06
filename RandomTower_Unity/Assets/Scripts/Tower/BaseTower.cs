using UnityEngine;

public class BaseTower : MonoBehaviour
{
    [SerializeField] public TowerData _data;
    public int Level { private set; get; }

    public float GetDamage => _data.BaseDamage + ((Level - 1) * 0.1f);
    public float GetRange => _data.BaseRange + ((Level - 1) * 0.1f);
    public float GetFireRate => _data.BaseFireRate + ((Level - 1) * 0.1f);

    public int ID => _data.ID;
    public int Grade => _data.Grade;
    public string TowerName => _data.TowerName;

    public void Initialize(TowerData data)
    {
        _data = data;
    }
}
