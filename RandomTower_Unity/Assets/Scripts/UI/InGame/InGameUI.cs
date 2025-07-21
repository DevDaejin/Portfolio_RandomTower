using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private InGameStatusUI _statusUI;
    [SerializeField] private InGameResultUI _resultUI;
    [SerializeField] private InGameTowerOptionUI _towerOptionUI;

    public Button WaveButton => _waveButton;
    [SerializeField] private Button _waveButton;
    public Button SpawnButton => _spawnButton;
    [SerializeField] private Button _spawnButton;
    public Button UpgradeButton => _upgradeButton;
    [SerializeField] private Button _upgradeButton;

    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TMP_Text _infoText;

    private RectTransform _infoPanelRectTransform;

    private EventTrigger _spawnTrigger;
    private EventTrigger _upgradeTrigger;

    private Func<int> _onUpdateSpawnPrice;
    private Func<int> _onUpdateUpgradePrice;
    private Func<int[]> _onUpdateUpgradeInfo;

    private enum ButtonType { Spawn, Upgrade }

    public void Initialize(int maxWave, int maxEnemy, int maxTower, float time, int gold, bool isHost, Action merge, Action sell, Func<int> onSpawnPrice, Func<int>onUpgradePrice, Func<int[]> onUpgradeInfo)
    {
        _resultUI.Intialize();
        _towerOptionUI.Initialize(merge, sell);        

        SetWave(0, maxWave);
        SetEnemyCount(0, maxEnemy);
        SetTimer(time);
        SetGoldCount(gold);
        SetTowerCount(0, maxTower);
        _resultUI.ActiveUI(false);

        UpgradeButton.onClick.RemoveAllListeners();
        SpawnButton.onClick.RemoveAllListeners();
        WaveButton.onClick.RemoveAllListeners();

        UpgradeButton.onClick.AddListener(() => UpdateInfoText(ButtonType.Upgrade));
        SpawnButton.onClick.AddListener(() => UpdateInfoText(ButtonType.Spawn));

        WaveButton.gameObject.SetActive(isHost);

        _onUpdateSpawnPrice = onSpawnPrice;
        _onUpdateUpgradePrice = onUpgradePrice;
        _onUpdateUpgradeInfo = onUpgradeInfo;

        _spawnTrigger ??= SpawnButton.GetComponent<EventTrigger>();
        _spawnTrigger.triggers.Clear();
        SetEventTrigger(_spawnTrigger, EventTriggerType.PointerEnter, ActiveSpawnInfoPanel);
        SetEventTrigger(_spawnTrigger, EventTriggerType.PointerExit, DeactiveInfoPanel);

        _upgradeTrigger ??= UpgradeButton.GetComponent<EventTrigger>();
        _upgradeTrigger.triggers.Clear();
        SetEventTrigger(_upgradeTrigger, EventTriggerType.PointerEnter, ActiveUpgradeInfoPanel);
        SetEventTrigger(_upgradeTrigger, EventTriggerType.PointerExit, DeactiveInfoPanel);
    }

    private void SetEventTrigger(EventTrigger trigger, EventTriggerType type, Action callback)
    {
        var entry = new EventTrigger.Entry { eventID = type};
        entry.callback.AddListener( _ => callback.Invoke());
        trigger.triggers.Add(entry);
    }

    private void ActiveSpawnInfoPanel()
    {
        ActiveInfoPanel();
        UpdateInfoText(ButtonType.Spawn);
    }

    private void ActiveUpgradeInfoPanel()
    {
        ActiveInfoPanel();
        UpdateInfoText(ButtonType.Upgrade);
    }

    private void UpdateInfoText(ButtonType type)
    {
        StringBuilder result = new();
        result.Clear();

        var array = _onUpdateUpgradeInfo.Invoke();

        result.AppendLine($"Grade 1 : {array[0]}%")
            .AppendLine($"Grade 2 : {array[1]}%")
            .AppendLine($"Grade 3 : {array[2]}%");

        switch (type)
        {
            case ButtonType.Spawn:
                result.AppendLine($"\n\nSpawn price : {_onUpdateSpawnPrice.Invoke()} Gold");
                break;
            case ButtonType.Upgrade:
                result.AppendLine($"\n\nUpgrade price : {_onUpdateUpgradePrice.Invoke()} Gold");
                break;
            default:
                result.Clear();
                break;
        }

        _infoText.text = result.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);
        _infoPanelRectTransform ??= _infoPanel.GetComponent<RectTransform>();
        _infoPanelRectTransform.sizeDelta = _infoText.rectTransform.sizeDelta;
        
    }
    private void ActiveInfoPanel() => _infoPanel.SetActive(true);
    private void DeactiveInfoPanel() => _infoPanel.SetActive(false);
    public void SetInteractableWaveButton(bool isAct) => WaveButton.interactable = isAct;
    public void ActiveTowerOptionMenuUI(bool isAct) => _towerOptionUI.ActiveUI(isAct);
    public void MoveTowerOptionMenuUI(Vector3 position) => _towerOptionUI.MoveUI(position);
    public void SetInterableMergeButton(bool isAct) => _towerOptionUI.SetInterableMergeButton(isAct);
    public void SetInterableUpgradeButton(bool isAct) => UpgradeButton.interactable = isAct;
    public void SetWave(int current, int max) => _statusUI.SetWave(current, max);
    public void SetGoldCount(int current) => _statusUI.SetGoldCount(current);
    public void SetTimer(float time) => _statusUI.SetTimer(time);
    public void SetEnemyCount(int current, int max) => _statusUI.SetEnemyCount(current, max);
    public void SetTowerCount(int current, int max) => _statusUI.SetTowerCount(current, max);
    public void SetResult(bool isSuccess) => _resultUI.SetResult(isSuccess);
    public void SetResultButtons(Action onRetry, Action onLobby) => _resultUI.SetResultButtons(onRetry, onLobby);
}