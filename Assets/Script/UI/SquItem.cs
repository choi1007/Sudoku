/// <summary>칸의 좌표와 정답은 변경할 수 없으며, 플레이어의 입력 상태는 세션에서만 변경합니다.</summary>
public sealed class SquItem
{
    public int Row { get; }
    public int Column { get; }
    public int Value { get; }
    public bool IsGiven { get; }
    public int InputValue { get; private set; }
    public bool IsHint { get; private set; }
    public bool Blank => InputValue != Value;

    internal SquItem(int row, int column, int value, bool isGiven)
    {
        Row = row; Column = column; Value = value; IsGiven = isGiven;
        InputValue = isGiven ? value : 0;
    }

    internal void SetInput(int number, bool hint)
    {
        if (!Blank) return;
        InputValue = number;
        IsHint = hint;
    }
}

namespace Event
{
    public static class EventUI
    {
        public struct EventClickClear { public int Number; }
        public struct EventInputNumCheck { public int Number; public int Remaining; }
        public struct EventCellChanged { public int Row; public int Column; }
    }
}
