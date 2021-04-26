using UnityEngine.Events;

public interface ISelectable
{
    UnityAction OnSelect { get; set; }
    UnityAction OnDeselect { get; set; }
}
