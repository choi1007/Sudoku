using System;

/// <summary>Independent searches own their board and row/column/box bit masks.</summary>
public sealed class SudokuSolver
{
    private const int AllDigits = 0x3FE;

    public int CountSolutions(int[,] board, int limit = 2)
    {
        if (limit < 1) throw new ArgumentOutOfRangeException(nameof(limit));
        return new Search(board, null).Run(limit);
    }

    public int[,] CreateSolution(Random random)
    {
        if (random == null) throw new ArgumentNullException(nameof(random));
        var search = new Search(new int[9, 9], random);
        if (search.Run(1) != 1) throw new InvalidOperationException("Could not generate a Sudoku solution.");
        return search.FirstSolution;
    }

    private sealed class Search
    {
        private readonly int[,] board;
        private readonly int[] rows = new int[9];
        private readonly int[] columns = new int[9];
        private readonly int[] boxes = new int[9];
        private readonly Random random;
        private bool valid = true;
        public int[,] FirstSolution { get; private set; }

        public Search(int[,] source, Random random)
        {
            if (source == null || source.GetLength(0) != 9 || source.GetLength(1) != 9)
                throw new ArgumentException("A Sudoku board must be 9 by 9.", nameof(source));
            board = (int[,])source.Clone();
            this.random = random;
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                {
                    int value = board[row, column];
                    if (value == 0) continue;
                    if (value < 1 || value > 9) { valid = false; continue; }
                    int bit = 1 << value;
                    int box = Box(row, column);
                    if (((rows[row] | columns[column] | boxes[box]) & bit) != 0) valid = false;
                    rows[row] |= bit;
                    columns[column] |= bit;
                    boxes[box] |= bit;
                }
        }

        public int Run(int limit) => valid ? Visit(limit) : 0;

        private int Visit(int limit)
        {
            int bestRow = -1, bestColumn = -1, bestMask = 0, bestCount = 10;
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                {
                    if (board[row, column] != 0) continue;
                    int mask = AllDigits & ~(rows[row] | columns[column] | boxes[Box(row, column)]);
                    int count = CountBits(mask);
                    if (count == 0) return 0;
                    if (count < bestCount)
                    {
                        bestRow = row; bestColumn = column; bestMask = mask; bestCount = count;
                    }
                }

            if (bestRow < 0)
            {
                if (random != null && FirstSolution == null) FirstSolution = (int[,])board.Clone();
                return 1;
            }

            // Local candidates cannot be overwritten by a recursive child.
            var candidates = new int[bestCount];
            int index = 0;
            for (int value = 1; value <= 9; value++)
                if ((bestMask & (1 << value)) != 0) candidates[index++] = value;
            if (random != null)
                for (int i = candidates.Length - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    int value = candidates[i]; candidates[i] = candidates[j]; candidates[j] = value;
                }

            int solutions = 0;
            int boxIndex = Box(bestRow, bestColumn);
            foreach (int value in candidates)
            {
                int bit = 1 << value;
                board[bestRow, bestColumn] = value;
                rows[bestRow] |= bit; columns[bestColumn] |= bit; boxes[boxIndex] |= bit;
                solutions += Visit(limit - solutions);
                board[bestRow, bestColumn] = 0;
                rows[bestRow] &= ~bit; columns[bestColumn] &= ~bit; boxes[boxIndex] &= ~bit;
                if (solutions >= limit) break;
            }
            return solutions;
        }

        private static int Box(int row, int column) => row / 3 * 3 + column / 3;
        private static int CountBits(int mask)
        {
            int count = 0;
            while (mask != 0) { mask &= mask - 1; count++; }
            return count;
        }
    }
}
