using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Transform SquMainGrid;
    [SerializeField] private List<Transform> InputGrid;
    [SerializeField] private Text TimeText;

    private StringBuilder StringBuilder = new StringBuilder();

    private List<BigSquItem> BigSquItem = new List<BigSquItem>(9);
    private List<NumInputItem> NumInputUIList = new List<NumInputItem>(9);

    private int displayedSeconds = -1;
    
    public void InitUI(SquItem[,] _squArray)
    {
        InitData();
        InitSquItem(_squArray);
        InitInputItem(_squArray);
    }

    private void InitData()
    {
        displayedSeconds = -1;
        TimeText.text = "00:00";
    }

    private void InitSquItem(SquItem[,] _squArray)
    {
        if (BigSquItem.Count > 0)
        {
            for(int _dialogIdx = 0; _dialogIdx < BigSquItem.Count; ++_dialogIdx)
            {
                BigSquItem[_dialogIdx].InitBigSqu(_squArray, _dialogIdx);
            }
            return;
        }

        for (int _dialogIdx = 0; _dialogIdx < 9; ++_dialogIdx)
        {
            var _createBigSqu = Util.GetPrefab<BigSquItem>("Prefabs/Item/BigSquItem");
            _createBigSqu.InitBigSqu(_squArray, _dialogIdx);
            _createBigSqu.transform.SetParent(SquMainGrid, false);
            BigSquItem.Add(_createBigSqu);
        }
    }

    private void InitInputItem(SquItem[,] _squArray)
    {
        if (NumInputUIList.Count > 0)
        {
            for(int _inputIdx = 0; _inputIdx < NumInputUIList.Count; ++_inputIdx)
            {
                NumInputUIList[_inputIdx].InitItem(_squArray, _inputIdx + 1);
            }
            return;
        }

        for (int _inputIdx = 0; _inputIdx < 9; ++_inputIdx)
        {
            var _createInputItem = Util.GetPrefab<NumInputItem>("Prefabs/Item/NumInputItem");
            _createInputItem.InitItem(_squArray, _inputIdx + 1);
            _createInputItem.transform.SetParent(InputGrid[_inputIdx / 3], false);
            NumInputUIList.Add(_createInputItem);
        }
    }

    public void OnClickOption()
    {
        OptionDialog.DoModal();
    }

    public void OnClickHint()
    {
        GameManager.Instance.BlankHint();
    }

    void LateUpdate()
    {
        int seconds = (int)GameManager.Instance.ElapsedSeconds;
        if (seconds == displayedSeconds) return;
        displayedSeconds = seconds;
        TimeText.text = TimeTextString(seconds);
    }

    private string TimeTextString(int _time)
    {
        StringBuilder.Clear();
        StringBuilder.AppendFormat("{0:00}:{1:00}", _time / 60, _time % 60);
        return StringBuilder.ToString();
    }
}
