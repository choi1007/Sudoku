using Event;
using UnityEngine;
using UnityEngine.UI;

public class SmallSquItem : MonoBehaviour
{
    [SerializeField] private Text NumText;
    [SerializeField] private Image BackImage;
    [SerializeField] private Button Button;
    private SquItem cell;
    public bool RightNum => cell != null && !cell.Blank;

    private void OnEnable() => EventAggregator.Instance.Subscribe<EventUI.EventCellChanged>(OnCellChanged);
    private void OnDisable() => EventAggregator.Instance.Unsubscribe<EventUI.EventCellChanged>(OnCellChanged);

    private void OnCellChanged(EventUI.EventCellChanged change)
    {
        if (cell != null && cell.Row == change.Row && cell.Column == change.Column) Refresh();
    }

    // Keep existing callers and prefab event bindings compatible.
    public void InitSamllSqu(SquItem data)
    {
        cell = data;
        Refresh();
    }

    private void Refresh()
    {
        Button.enabled = true;
        Button.interactable = cell.Blank;
        NumText.text = cell.InputValue == 0 ? string.Empty : cell.InputValue.ToString();
        NumText.color = cell.IsGiven || cell.InputValue == 0 ? Color.black
            : cell.IsHint ? Color.magenta : cell.Blank ? Color.red : Color.blue;
    }

    public void OnClickSqu()
    {
        if (cell != null) GameManager.Instance.EnterNumber(cell.Row, cell.Column);
    }
}
