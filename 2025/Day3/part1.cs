if (args.Length > 0 && args[0] == "--test")
{
    var b1 = "987654321111111";
    var b2 = "811111111111119";
    var b3 = "234234234234278";
    var b4 = "818181911112111";
    var puzzleInput = string.Join("\n", [b1, b2, b3, b4]);

    var b1ExpectedJoltage = 98;
    var b2ExpectedJoltage = 89;
    var b3ExpectedJoltage = 78;
    var b4ExpectedJoltage = 92;
    int puzzleExpectedJoltage = 357;

    TestBank(b1, b1ExpectedJoltage);
    TestBank(b2, b2ExpectedJoltage);
    TestBank(b3, b3ExpectedJoltage);
    TestBank(b4, b4ExpectedJoltage);
    TestPuzzle(puzzleInput, puzzleExpectedJoltage);
} 
else
{
    var input = System.IO.File.ReadAllText("input.txt");
    Console.WriteLine($"Password: {Puzzle.FromString(input).GetPassword()}");
}

const string COLOR_RESET = "\x1b[0m";
const string COLOR_GREEN = "\x1b[92m";
const string COLOR_RED = "\x1b[91m";

void TestPuzzle(string givenInput, int expectedJoltage)
{
    int password = Puzzle.FromString(givenInput).GetPassword();

    if (password == expectedJoltage)
    {
        Console.WriteLine($"Puzzle {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Puzzle {COLOR_RED}failed{COLOR_RESET}. Expected {expectedJoltage}, got {password}.");
    }
}

void TestBank(string batteries, int expectedJoltage)
{
    int largestPossibleJoltage = new Bank(batteries).GetLargestPossibleJoltage();

    if (largestPossibleJoltage == expectedJoltage)
    {
        Console.WriteLine($"Test {batteries} {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test {batteries} {COLOR_RED}failed{COLOR_RESET}. Expected {expectedJoltage}, got {largestPossibleJoltage}.");
    }
}

class Puzzle
{
    private List<Bank> Banks = [];

    public static Puzzle FromString(string puzzleStr)
    {
        var banks = puzzleStr.Split('\n').Select(batteries => new Bank(batteries)).ToList();
        return new Puzzle { Banks = banks };
    }

    public int GetPassword()
    {
        return Banks.Select(bank => bank.GetLargestPossibleJoltage()).Sum();
    }
}

class Bank(string batteries)
{
    private string batteries = batteries;

    public int GetLargestPossibleJoltage()
    {
        int maxJoltage = 0;

        for (int i = 0; i < batteries.Length - 1; i++)
        {
            for (int j = i + 1; j < batteries.Length; j++)
            {
                int joltage = int.Parse(batteries[i].ToString() + batteries[j].ToString());
                if (joltage > maxJoltage)
                {
                    maxJoltage = joltage;
                }
            }
        }

        return maxJoltage;
    }

    public override string ToString()
    {
        return batteries;
    }
}
