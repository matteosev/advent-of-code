if (args.Length > 0 && args[0] == "--test")
{
    TestPuzzle(System.IO.File.ReadAllText("test-input.txt"), 13);
} 
else
{
    var input = System.IO.File.ReadAllText("input.txt");
    Console.WriteLine($"Password: {Puzzle.FromString(input).GetPassword()}");
}

const string COLOR_RESET = "\x1b[0m";
const string COLOR_GREEN = "\x1b[92m";
const string COLOR_RED = "\x1b[91m";

void TestPuzzle(string givenInput, long expectedPassword)
{
    long password = Puzzle.FromString(givenInput).GetPassword();

    if (password == expectedPassword)
    {
        Console.WriteLine($"Puzzle {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Puzzle {COLOR_RED}failed{COLOR_RESET}. Expected {expectedPassword}, got {password}.");
    }
}

class Puzzle
{
    private string[] grid = [];

    public static Puzzle FromString(string puzzleStr)
    {
        return new Puzzle { grid = puzzleStr.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToArray() };
    }

    public long GetPassword()
    {
        int nbRows = grid.Length;
        int nbCols = grid[0].Length;
        long nbOfAccessibleRolls = 0;
        const int MAX_NB_ADJACENT_ROLLS_TO_BE_ACCESSIBLE = 3;

        for (int row = 0; row < nbRows; row++)
        {
            for (int col = 0; col < nbCols; col++)
            {
                List<char> adjacentCells = [];
                bool isNotOnFirstRow = row > 0;
                bool isNotOnLastRow = row < nbRows - 1;
                bool isNotOnFirstCol = col > 0;
                bool isNotOnLastCol = col < nbCols - 1;

                int precedentRow = row - 1;
                int nextRow = row + 1;
                int precedentCol = col - 1;
                int nextCol = col + 1;

                if (isNotOnFirstRow)
                {
                    adjacentCells.Add(grid[precedentRow][col]);
                }
                if (isNotOnLastRow)
                {
                    adjacentCells.Add(grid[nextRow][col]);
                }
                if (col > 0)
                {
                    adjacentCells.Add(grid[row][precedentCol]);
                }
                if (col < nbCols - 1)
                {
                    adjacentCells.Add(grid[row][nextCol]);
                }
                if (isNotOnFirstRow && isNotOnFirstCol)
                {
                    adjacentCells.Add(grid[precedentRow][precedentCol]);
                }
                if (isNotOnFirstRow && isNotOnLastCol)
                {
                    adjacentCells.Add(grid[precedentRow][nextCol]);
                }
                if (isNotOnLastRow && isNotOnFirstCol)
                {
                    adjacentCells.Add(grid[nextRow][precedentCol]);
                }
                if (isNotOnLastRow && isNotOnLastCol)
                {
                    adjacentCells.Add(grid[nextRow][nextCol]);
                }

                int nbAdjacentRolls = adjacentCells.Count(c => c == '@');

                if (nbAdjacentRolls <= MAX_NB_ADJACENT_ROLLS_TO_BE_ACCESSIBLE && grid[row][col] == '@')
                {
                    nbOfAccessibleRolls++;
                }

                // Console.WriteLine($"Cell ({row},{col})='{grid[row][col]}' has adjacent cells: [{string.Join(",", adjacentCells)}] nbAdjacentRolls={nbAdjacentRolls}");
            }
        }

        return nbOfAccessibleRolls;
    }
}
