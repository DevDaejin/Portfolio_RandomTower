using System;
using UnityEngine;

public class MessageBox
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PositiveButtonText { get; set; } = string.Empty;
    public string NegativeButtonText { get; set; } = string.Empty;
    public Action OnPositiveButtonClick { get; set; } = null;
    public Action OnNegativeButtonClick { get; set; } = null;
    public Vector2 MessageBoxSize { get; set; } = Vector2.zero;
    public Vector2 DescriptionSize { get; set; } = Vector2.zero;
}
