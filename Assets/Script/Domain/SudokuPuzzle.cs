using System;

/// <summary>플레이어의 진행 상태와 분리하여 초기 문제와 정답을 변경 불가능한 형태로 보관합니다.</summary>
public sealed class SudokuPuzzle
{
    private readonly int[,] solution;
    private readonly int[,] clues;
    public int Level { get; }
    public int BlankCount { get; }

    internal SudokuPuzzle(int[,] solution, int[,] clues, int level)
    {
        this.solution = (int[,])solution.Clone();
        this.clues = (int[,])clues.Clone();
        Level = level;
        foreach (int value in clues) if (value == 0) BlankCount++;
    }

    public int GetSolution(int row, int column) => solution[row, column];
    public bool IsGiven(int row, int column) => clues[row, column] != 0;
    public int[,] CopyClues() => (int[,])clues.Clone();
}
