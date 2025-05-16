using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _enemyCountText;

    [SerializeField] private Button _spawnButton;

    private StringBuilder _timerStringBuilder = new();
    private StringBuilder _waveStringBuilder = new();
    private StringBuilder _enemyStringBuilder = new();

    public void SetWave(int current, int max)
    {
        _waveStringBuilder.Clear();
        _waveStringBuilder.Append("Wave\n").Append($"{current} / {max}");
        _waveText.text = _waveStringBuilder.ToString();
    }

    public void SetTimer(float time)
    {
        int second = Mathf.CeilToInt(time);

        _timerStringBuilder.Clear();
        _timerStringBuilder.Append("Time\n").Append("{second:00}");
        _timerText.text = _timerStringBuilder.ToString();
    }

    public void SetEnemyCount(int current, int max)
    {
        _enemyStringBuilder.Clear();
        _enemyStringBuilder.Append("Enemy\n").Append($"{current} / {max}");
        _enemyCountText.text = _enemyStringBuilder.ToString();
    }

    public void SetSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.AddListener(callback);
    }

    public void ReleaseSpawnButton(UnityAction callback)
    {
        _spawnButton.onClick.RemoveListener(callback);
    }
}
