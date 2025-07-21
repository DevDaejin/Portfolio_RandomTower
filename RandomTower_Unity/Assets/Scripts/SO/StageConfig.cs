using UnityEngine;

[CreateAssetMenu(fileName = "StageConfig", menuName = "Random TD/StageConfig")]
public class StageConfig : ScriptableObject
{
    public int StageLevel => _stageLevel;
    [SerializeField] private int _stageLevel;

    public int ClearReward;
    public int FailedReward;

    public WaveData WaveData => _waveData;
    [SerializeField] private WaveData _waveData;
}
