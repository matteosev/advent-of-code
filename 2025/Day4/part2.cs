if (args.Length > 0 && args[0] == "--test")
{
    TestPuzzle(System.IO.File.ReadAllText("test-input.txt"), 43);
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
        long totalNbOfAccessibleRolls = 0;

        while (ComputeNbOfAccessibleRolls() > 0)
        {
            totalNbOfAccessibleRolls += ComputeNbOfAccessibleRolls();

            // Remove accessible rolls
            int nbRows = grid.Length;
            int nbCols = grid[0].Length;
            char[][] newGrid = grid.Select(line => line.ToCharArray()).ToArray();

            for (int row = 0; row < nbRows; row++)
            {
                for (int col = 0; col < nbCols; col++)
                {
                    if (grid[row][col] == '@' && IsAccessible(row, col))
                    {
                        newGrid[row][col] = '.';
                    }
                }
            }

            grid = newGrid.Select(line => new string(line)).ToArray();
        }
        
        return totalNbOfAccessibleRolls;
    }

    private long ComputeNbOfAccessibleRolls()
    {
        int nbRows = grid.Length;
        int nbCols = grid[0].Length;
        long nbOfAccessibleRolls = 0;

        for (int row = 0; row < nbRows; row++)
        {
            for (int col = 0; col < nbCols; col++)
            {
                if (grid[row][col] != '@')
                {
                    continue; // Only consider cells with a roll '@'
                }

                if (IsAccessible(row, col))
                {
                    nbOfAccessibleRolls++;
                }

                // Console.WriteLine($"Cell ({row},{col})='{grid[row][col]}' has adjacent cells: [{string.Join(",", adjacentCells)}] nbAdjacentRolls={nbAdjacentRolls}");
            }
        }

        return nbOfAccessibleRolls;
    }

    private bool IsAccessible(int row, int col)
    {
        int nbRows = grid.Length;
        int nbCols = grid[0].Length;
        int nbAdjacentRolls = 0;

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                if (r == row && c == col)
                {
                    continue; // Skip the current cell
                }

                if (r >= 0 && r < nbRows && c >= 0 && c < nbCols)
                {
                    if (grid[r][c] == '@')
                    {
                        nbAdjacentRolls++;
                    }
                }
            }
        }

        return nbAdjacentRolls < 4;
    }
}

record Position(int Row, int Col);