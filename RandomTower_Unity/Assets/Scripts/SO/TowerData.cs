using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Random TD/TowerData")]
public class TowerData : ScriptableObject
{
    public int Level;

    [SerializeField] private int grade;
    public int Grade => grade;

    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string towerName;
    public string TowerName => towerName;

    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField] private float baseDamage;
    public float BaseDamage => baseDamage;

    [SerializeField] private float baseRange;
    public float BaseRange => baseRange;

    [SerializeField] private float baseFireRate;
    public float BaseFireRate => baseFireRate;

    [SerializeField] private bool isSpecial;
    public bool IsSpecial => isSpecial;
}
