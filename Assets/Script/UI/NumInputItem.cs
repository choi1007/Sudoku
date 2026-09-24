using UnityEngine;
using UnityEngine.UI;
using Event;

public class NumInputItem : MonoBehaviour
{
    [SerializeField] private Image NumberInputImage;
    [SerializeField] private Text NumberInpuText;
    private int Number;
    private bool Clear;
    private Button button;

    private void Awake() => button = GetComponent<Button>();
    private void OnEnable()
    {
        EventAggregator.Instance.Subscribe<EventUI.EventClickClear>(EventClickEvent);
        EventAggregator.Instance.Subscribe<EventUI.EventInputNumCheck>(EventNumberFull);
    }
    private void OnDisable()
    {
        EventAggregator.Instance.Unsubscribe<EventUI.EventClickClear>(EventClickEvent);
        EventAggregator.Instance.Unsubscribe<EventUI.EventInputNumCheck>(EventNumberFull);
    }
    private void EventClickEvent(EventUI.EventClickClear change)
    {
        NumberInputImage.color = Clear ? Color.black : change.Number == Number ? Color.gray : Color.white;
    }
    private void EventNumberFull(EventUI.EventInputNumCheck change)
    {
        if (change.Number != Number) return;
        Clear = change.Remaining == 0;
        if (button != null) button.interactable = !Clear;
    }
    public void InitItem(SquItem[,] unused, int number)
    {
        Clear = false;
        Number = number;
        NumberInpuText.text = Number.ToString();
        NumberInputImage.color = Color.white;
        if (button != null) button.interactable = true;
    }
    public void OnClickInput()
    {
        if (!Clear) GameManager.Instance.SelectNumber(Number);
    }
}
