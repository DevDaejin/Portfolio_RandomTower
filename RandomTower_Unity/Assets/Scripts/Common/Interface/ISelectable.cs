using UnityEngine;

public interface ISelectable
{
    GameObject Selectd { get; }
    void OnSelect();
    void OnDeselect();
}
