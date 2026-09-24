using System;

/// <summary>Generates a solved board, then removes clues only while the solution remains unique.</summary>
public sealed class SudokuManager
{
    private readonly int level;
    private readonly Random random;
    private readonly SudokuSolver solver = new SudokuSolver();

    public SudokuManager(int level) : this(level, new Random()) { }
    public SudokuManager(int level, Random random)
    {
        this.level = Math.Max(1, Math.Min(7, level));
        this.random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public SudokuPuzzle Generate()
    {
        int[,] solution = solver.CreateSolution(random);
        int[,] clues = (int[,])solution.Clone();
        var positions = new int[81];
        for (int i = 0; i < positions.Length; i++) positions[i] = i;
        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int value = positions[i]; positions[i] = positions[j]; positions[j] = value;
        }

        // Levels retain the original approximate blank ratio, not a logical difficulty rating.
        // If uniqueness prevents the target, keep more clues instead of accepting ambiguity.
        int targetBlanks = 81 * level / 10;
        int removed = 0;
        foreach (int position in positions)
        {
            int row = position / 9, column = position % 9;
            int value = clues[row, column];
            clues[row, column] = 0;
            if (solver.CountSolutions(clues) == 1) removed++;
            else clues[row, column] = value;
            if (removed >= targetBlanks) break;
        }
        return new SudokuPuzzle(solution, clues, level);
    }
}
