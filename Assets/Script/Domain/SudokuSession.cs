using System;
using System.Collections.Generic;

public enum SudokuGameState { Playing, Completed }
public enum SudokuInputResult { Ignored, Incorrect, Correct }

/// <summary>Owns all mutable game state; UI only reads cells and submits commands.</summary>
public sealed class SudokuSession
{
    private readonly SquItem[,] cells = new SquItem[9, 9];
    private readonly HashSet<int> unresolved = new HashSet<int>();
    private readonly int[] remainingByNumber = new int[10];
    public SudokuGameState State { get; private set; }
    public int SelectedNumber { get; private set; }
    public double ElapsedSeconds { get; private set; }
    public int RemainingCount => unresolved.Count;

    public SudokuSession(SudokuPuzzle puzzle)
    {
        if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));
        for (int row = 0; row < 9; row++)
            for (int column = 0; column < 9; column++)
            {
                var cell = new SquItem(row, column, puzzle.GetSolution(row, column), puzzle.IsGiven(row, column));
                cells[row, column] = cell;
                if (!cell.Blank) continue;
                unresolved.Add(row * 9 + column);
                remainingByNumber[cell.Value]++;
            }
        State = unresolved.Count == 0 ? SudokuGameState.Completed : SudokuGameState.Playing;
    }

    public SquItem GetCell(int row, int column) => cells[row, column];
    public SquItem[,] CopyCells() => (SquItem[,])cells.Clone();
    public int RemainingFor(int number) => number >= 1 && number <= 9 ? remainingByNumber[number] : 0;

    public bool SelectNumber(int number)
    {
        if (State != SudokuGameState.Playing || RemainingFor(number) == 0) return false;
        SelectedNumber = SelectedNumber == number ? 0 : number;
        return true;
    }

    public SudokuInputResult Enter(int row, int column, int number)
    {
        return Apply(row, column, number, false);
    }

    public bool TryHint(Random random, out SquItem cell)
    {
        cell = null;
        if (State != SudokuGameState.Playing || unresolved.Count == 0) return false;
        if (random == null) throw new ArgumentNullException(nameof(random));
        int skip = random.Next(unresolved.Count);
        int selected = -1;
        // Enumerate at most 81 entries, without allocating a filtered list.
        foreach (int position in unresolved)
        {
            if (skip-- == 0) { selected = position; break; }
        }
        cell = cells[selected / 9, selected % 9];
        Apply(cell.Row, cell.Column, cell.Value, true);
        return true;
    }

    public void AdvanceTime(double seconds, bool paused)
    {
        if (State != SudokuGameState.Playing || paused || seconds <= 0 || double.IsNaN(seconds) || double.IsInfinity(seconds)) return;
        ElapsedSeconds += seconds;
    }

    private SudokuInputResult Apply(int row, int column, int number, bool hint)
    {
        if (State != SudokuGameState.Playing || row < 0 || row >= 9 || column < 0 || column >= 9 || number < 1 || number > 9)
            return SudokuInputResult.Ignored;
        var cell = cells[row, column];
        if (!cell.Blank) return SudokuInputResult.Ignored;
        cell.SetInput(number, hint);
        if (cell.Blank) return SudokuInputResult.Incorrect;
        unresolved.Remove(row * 9 + column);
        remainingByNumber[cell.Value]--;
        if (RemainingFor(SelectedNumber) == 0) SelectedNumber = 0;
        if (unresolved.Count == 0) State = SudokuGameState.Completed;
        return SudokuInputResult.Correct;
    }
}
