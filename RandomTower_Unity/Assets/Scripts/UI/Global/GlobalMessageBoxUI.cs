using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalMessageBoxUI : MonoBehaviour
{
    public GameObject Panel => _meesageBoxPanel;
    [SerializeField] private GameObject _meesageBoxPanel;
    [SerializeField] private RectTransform _messageBoxArea;
    [SerializeField] private TMP_Text _titleTxt;
    [SerializeField] private RectTransform _descriptionArea;
    [SerializeField] private TMP_Text _descriptionTxt;
    [SerializeField] private Button _positiveButton;
    [SerializeField] private Button _negativeButton;

    private TMP_Text _positiveButtonTxt;
    private TMP_Text _negativeButtonTxt;

    private Action _onPositiveCallback;
    private Action _onNegativeCallback;

    private readonly Vector2 BasicMessageBoxSize = new Vector2(920, 680);
    private readonly Vector2 BasicDescriptionSize = new Vector2(700, 250);

    private void Awake()
    {
        _positiveButtonTxt = _positiveButton.GetComponentInChildren<TMP_Text>();
        _negativeButtonTxt = _negativeButton.GetComponentInChildren<TMP_Text>();

        _positiveButton.onClick.AddListener(() =>
        {
            _onPositiveCallback?.Invoke();
            ActiveUI(false);
        });

        _negativeButton.onClick.AddListener(() =>
        {
            _onNegativeCallback?.Invoke();
            ActiveUI(false);
        });
    }

    public void ShowContext(MessageBoxData box)
    {
        Resize(box.MessageBoxSize, box.DescriptionSize);
        SetTitle(box.Title);
        SetDescription(box.Description);
        SetButtons(box.PositiveButtonText, box.NegativeButtonText, box.OnPositiveButtonClick, box.OnNegativeButtonClick);

        ActiveUI(true);
    }

    private void Resize(Vector2 messageBoxSize, Vector2 descriptionSize)
    {
        if (messageBoxSize != Vector2.zero)
        {
            SetSizeDelta(_messageBoxArea, messageBoxSize);
        }

        if (descriptionSize != Vector2.zero)
        {
            SetSizeDelta(_descriptionArea, descriptionSize);
        }
    }


    private void SetSizeDelta(RectTransform target, Vector2 size)
    {
        var endSize = size == Vector2.zero ? GetBasicSize(target) : size;
        
        var originPivot = target.pivot;
        var originAnchorMin = target.anchorMin;
        var originAnchorMax = target.anchorMax;

        var center = Vector2.one * 0.5f;
        SetAnchorAndPivot(target, center, center, center);
        target.sizeDelta = endSize;
        SetAnchorAndPivot(target, originAnchorMin, originAnchorMax, originPivot);
    }

    private void SetAnchorAndPivot(RectTransform target, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        target.anchorMin = anchorMin;
        target.anchorMax = anchorMax;
        target.pivot = pivot;
    }

    private Vector2 GetBasicSize(RectTransform target)
    {
        Vector2 size = Vector2.zero;
        if(target == _messageBoxArea)
        {
            size = BasicMessageBoxSize;
        }

        else if (target == _descriptionArea)
        {
            size = BasicDescriptionSize;
        }

        return BasicMessageBoxSize;
    }


    private void SetTitle(string title)
    {
        SetText(_titleTxt, title);
    }

    private void SetDescription(string description)
    {
        SetText(_descriptionTxt, description);
    }

    private void SetButtons(string positiveText, string negativeText, Action onPositiveCallback, Action onNegativeCallback)
    {
        SetText(_positiveButtonTxt, positiveText);
        SetText(_negativeButtonTxt, negativeText);

        if (onPositiveCallback != null) _onPositiveCallback = onPositiveCallback;
        if (onNegativeCallback != null) _onNegativeCallback = onNegativeCallback;
    }

    private void SetText(TMP_Text target, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            target.gameObject.SetActive(false);
            return;
        }

        target.gameObject.SetActive(true);
        target.text = text;
    }

    public void ActiveUI(bool isAct) => _meesageBoxPanel.SetActive(isAct);
}
