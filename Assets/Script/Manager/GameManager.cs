using Event;
using UnityEngine;

/// <summary>Coordinates the domain session, prefab UI, and existing UI event bus.</summary>
public class GameManager : Singleton<GameManager>
{
    public int SudokuLevel = 1;
    public int ClickNum => session == null ? 0 : session.SelectedNumber;
    public bool IsPlaying => session != null && session.State == SudokuGameState.Playing;
    public double ElapsedSeconds => session == null ? 0 : session.ElapsedSeconds;

    private MainUI m_mainUI;
    private SudokuSession session;
    private readonly System.Random random = new System.Random();
    private bool completionShown;

    public void OnWake() { }

    public void GameStart()
    {
        session = new SudokuSession(new SudokuManager(SudokuLevel, random).Generate());
        completionShown = false;
        if (m_mainUI == null)
        {
            m_mainUI = Util.GetPrefab<MainUI>("Prefabs/UI/MainUI");
            m_mainUI.transform.SetParent(UI.MainPanel, false);
        }
        m_mainUI.InitUI(session.CopyCells());
        PublishNumberState();
    }

    public void SelectNumber(int number)
    {
        if (DialogManager.Instance.Open || session == null || !session.SelectNumber(number)) return;
        PublishNumberState();
    }

    public void EnterNumber(int row, int column)
    {
        if (DialogManager.Instance.Open || session == null) return;
        if (session.Enter(row, column, ClickNum) == SudokuInputResult.Ignored) return;
        OnCellChanged(session.GetCell(row, column));
    }

    public void BlankHint()
    {
        if (DialogManager.Instance.Open || session == null || !session.TryHint(random, out var cell)) return;
        OnCellChanged(cell);
    }

    private void Update()
    {
        if (session != null) session.AdvanceTime(Time.deltaTime, DialogManager.Instance.Open);
    }

    private void OnCellChanged(SquItem cell)
    {
        EventAggregator.Instance.Publish(new EventUI.EventCellChanged { Row = cell.Row, Column = cell.Column });
        PublishNumberState();
        if (session.State != SudokuGameState.Completed || completionShown) return;
        completionShown = true;
        ClearGameDialog.DoModal();
    }

    private void PublishNumberState()
    {
        for (int number = 1; number <= 9; number++)
            EventAggregator.Instance.Publish(new EventUI.EventInputNumCheck { Number = number, Remaining = session.RemainingFor(number) });
        EventAggregator.Instance.Publish(new EventUI.EventClickClear { Number = ClickNum });
    }
}
