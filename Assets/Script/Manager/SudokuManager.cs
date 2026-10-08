using System;

/// <summary>완성된 정답 보드를 생성한 뒤, 해답이 하나로 유지되는 경우에만 단서 숫자를 제거합니다.</summary>
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

        // 레벨은 기존의 대략적인 빈칸 비율을 유지하며, 논리적 풀이 난이도를 평가한 값은 아닙니다.
        // 목표 빈칸 수에서 유일해를 유지할 수 없다면 단서 숫자를 더 남깁니다.
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
