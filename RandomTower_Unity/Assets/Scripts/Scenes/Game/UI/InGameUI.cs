using System;
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

    [SerializeField] private Button _menuButton;
    public Button WaveButton => _waveButton;
    [SerializeField] private Button _waveButton;
    public Button SpawnButton => _spawnButton;
    [SerializeField] private Button _spawnButton;
    public Button UpgradeButton => _upgradeButton;
    [SerializeField] private Button _upgradeButton;

    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TMP_Text _infoText;

    private RectTransform _infoPanelRectTransform;

    private InGameUISetting _setting;

    private EventTrigger _spawnTrigger;
    private EventTrigger _upgradeTrigger;

    private enum ButtonType { Spawn, Upgrade }

    public void Initialize(InGameUISetting setting)
    {
        _setting = setting;

        _resultUI.Intialize(_setting.OnSuccessReward, _setting.OnFailedReward);
        _towerOptionUI.Initialize(_setting.OnMerge, _setting.OnSell);        

        SetWave(0, _setting.MaxWave);
        SetEnemyCount(0, _setting.MaxEnemy);
        SetTimer(_setting.Time);
        SetGoldCount(_setting.Gold);
        SetTowerCount(0, _setting.MaxTower);
        _resultUI.ActiveUI(false);

        UpgradeButton.onClick.RemoveAllListeners();
        SpawnButton.onClick.RemoveAllListeners();
        WaveButton.onClick.RemoveAllListeners();
        _menuButton.onClick.RemoveAllListeners();

        WaveButton.gameObject.SetActive(_setting.IsHost);
        _menuButton.onClick.AddListener(() => setting?.OnMenu?.Invoke());

        _spawnTrigger ??= SpawnButton.GetComponent<EventTrigger>();
        SetEventTriggers(_spawnTrigger, ActiveSpawnInfoPanel, DeactiveInfoPanel);

        _upgradeTrigger ??= UpgradeButton.GetComponent<EventTrigger>();
        SetEventTriggers(_upgradeTrigger, ActiveUpgradeInfoPanel, DeactiveInfoPanel);
    }

    private void SetEventTriggers(EventTrigger trigger, Action onActive, Action onDeactive)
    {
        trigger.triggers.Clear();
        SetEventTrigger(trigger, EventTriggerType.PointerEnter, onActive);
        SetEventTrigger(trigger, EventTriggerType.PointerUp, onActive);

        SetEventTrigger(trigger, EventTriggerType.PointerExit, onDeactive);
        SetEventTrigger(trigger, EventTriggerType.PointerDown, onDeactive);
    }

    private void SetEventTrigger(EventTrigger trigger, EventTriggerType type, Action callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => callback.Invoke());
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
        result = UpdateInfoProbability(result, _setting.OnUpgradeProbability.Invoke());
        result = UpdateInfoPrice(result, type);
        _infoText.text = result.ToString();
     
        UpdateInfoPanel();
    }

    private StringBuilder UpdateInfoProbability(StringBuilder builder, int[] array)
    {
        for (int index = 0; index < array.Length; index++)
        {
            builder.AppendLine($"Grade {index + 1} : {array[index]}%");
        }

        return builder;
    }

    private StringBuilder UpdateInfoPrice(StringBuilder builder, ButtonType type)
    {
        switch (type)
        {
            case ButtonType.Spawn:
                builder.AppendLine($"\n\nSpawn price : {_setting.OnSpawnPrice.Invoke()} Gold");
                break;
            case ButtonType.Upgrade:
                var price = _setting.OnUpgradePrice.Invoke();
                var priceText = price == 0 ? "-" : price.ToString();
                builder.AppendLine($"\n\nUpgrade price : {priceText} Gold");
                break;
            default:
                builder.Clear();
                break;
        }

        return builder;
    }

    private void UpdateInfoPanel()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_infoText.rectTransform);
        _infoPanelRectTransform ??= _infoPanel.GetComponent<RectTransform>();
        _infoPanelRectTransform.sizeDelta = _infoText.rectTransform.sizeDelta;
    }

    public void SetResult(bool isSuccess)
    {
        _resultUI.SetResult(isSuccess);
    }

    private void ActiveInfoPanel() => _infoPanel.SetActive(true);
    private void DeactiveInfoPanel() => _infoPanel.SetActive(false);
    public void SetInteractableWaveButton(bool isAct) => WaveButton.interactable = isAct;
    public void ActiveTowerOptionMenuUI(bool isAct) => _towerOptionUI.ActiveUI(isAct);
    public void MoveTowerOptionMenuUI(Vector3 position) => _towerOptionUI.MoveUI(position);
    public void SetInteractableMergeButton(bool isAct) => _towerOptionUI.SetInterableMergeButton(isAct);
    public void SetInteractableUpgradeButton(bool isAct) => UpgradeButton.interactable = isAct;
    public void SetWave(int current, int max) => _statusUI.SetWave(current, max);
    public void SetGoldCount(int current) => _statusUI.SetGoldCount(current);
    public void SetTimer(float time) => _statusUI.SetTimer(time);
    public void SetEnemyCount(int current, int max) => _statusUI.SetEnemyCount(current, max);
    public void SetTowerCount(int current, int max) => _statusUI.SetTowerCount(current, max);
    public void SetResultButtons(Action onRetry, Action onLobby) => _resultUI.SetResultButtons(onRetry, onLobby);
}