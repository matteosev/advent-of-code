if (args.Length > 0 && args[0] == "--test")
{
    var b1 = "987654321111111";
    var b2 = "811111111111119";
    var b3 = "234234234234278";
    var b4 = "818181911112111";
    var puzzleInput = string.Join("\n", [b1, b2, b3, b4]);

    var b1ExpectedJoltage = 987654321111;
    var b2ExpectedJoltage = 811111111119;
    var b3ExpectedJoltage = 434234234278;
    var b4ExpectedJoltage = 888911112111;
    long puzzleExpectedJoltage = 3121910778619;

    TestBank(b1, b1ExpectedJoltage);
    TestBank(b2, b2ExpectedJoltage);
    TestBank(b3, b3ExpectedJoltage);
    TestBank(b4, b4ExpectedJoltage);
    TestPuzzle(puzzleInput, puzzleExpectedJoltage);
    TestHelper();
} 
else
{
    var input = System.IO.File.ReadAllText("input.txt");
    Console.WriteLine($"Password: {Puzzle.FromString(input).GetPassword()}");
}

const string COLOR_RESET = "\x1b[0m";
const string COLOR_GREEN = "\x1b[92m";
const string COLOR_RED = "\x1b[91m";

void TestPuzzle(string givenInput, long expectedJoltage)
{
    long password = Puzzle.FromString(givenInput).GetPassword();

    if (password == expectedJoltage)
    {
        Console.WriteLine($"Puzzle {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Puzzle {COLOR_RED}failed{COLOR_RESET}. Expected {expectedJoltage}, got {password}.");
    }
}

void TestBank(string batteries, long expectedJoltage)
{
    long largestPossibleJoltage = new Bank(batteries).GetLargestPossibleJoltage();

    if (largestPossibleJoltage == expectedJoltage)
    {
        Console.WriteLine($"Test Bank {batteries} {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test Bank {batteries} {COLOR_RED}failed{COLOR_RESET}. Expected {expectedJoltage}, got {largestPossibleJoltage}.");
    }
}

void TestHelper()
{
    TestDigitsArrayIsFull();
    TestAppendDigit();
    TestDeleteAt();
}

void TestDigitsArrayIsFull()
{
    var fullArr = new int[] {9, 8, 7, 6, 5};
    var notFullArr = new int[] {9, 8, 0, 6, 5};

    if (DigitsArrayHelper.IsFull(fullArr) && !DigitsArrayHelper.IsFull(notFullArr))
    {
        Console.WriteLine($"Test DigitsArrayIsFull {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test DigitsArrayIsFull {COLOR_RED}failed{COLOR_RESET}.");
    }
}

void TestAppendDigit()
{
    var arr = new int[] {9, 8, 0, 0, 0};

    DigitsArrayHelper.Append(arr, 7);
    DigitsArrayHelper.Append(arr, 6);

    var expectedArr = new int[] {9, 8, 7, 6, 0};

    if (arr.SequenceEqual(expectedArr))
    {
        Console.WriteLine($"Test AppendDigit {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test AppendDigit {COLOR_RED}failed{COLOR_RESET}. Expected [{string.Join(",", expectedArr)}], got [{string.Join(",", arr)}].");
    }
}

void TestDeleteAt()
{
    var arr = new int[] {9, 8, 7, 6, 5};

    DigitsArrayHelper.DeleteAt(arr, 2);

    var expectedArr = new int[] {9, 8, 6, 5, 0};

    if (arr.SequenceEqual(expectedArr))
    {
        Console.WriteLine($"Test DeleteAt {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test DeleteAt {COLOR_RED}failed{COLOR_RESET}. Expected [{string.Join(",", expectedArr)}], got [{string.Join(",", arr)}].");
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

    public long GetPassword()
    {
        return Banks.Select(bank => bank.GetLargestPossibleJoltage()).Sum();
    }
}

class Bank(string batteries)
{
    private string batteries = batteries;

    public long GetLargestPossibleJoltage()
    {
        const int JOLTAGE_NB_DIGITS = 12;
        long maxJoltage = 0;

        for (int i = 0; i < batteries.Length - JOLTAGE_NB_DIGITS; i++)
        {
            var biggestDigits = new int[JOLTAGE_NB_DIGITS]; // Filled with zeros by default
            const int INDEX_OF_LAST_DIGIT = JOLTAGE_NB_DIGITS - 1;

            for (int j = i; j < batteries.Length; j++)
            {
                int currentDigit = int.Parse(batteries[j].ToString());

                if (DigitsArrayHelper.IsFull(biggestDigits))
                {
                    int indexOfFirstDigitSmallerThanNextDigit = DigitsArrayHelper.FindFirstDigitSmallerThanNext(biggestDigits);
                    
                    int indexOfDigitToDelete = -1;

                    if (indexOfFirstDigitSmallerThanNextDigit != -1)
                    {
                        indexOfDigitToDelete = indexOfFirstDigitSmallerThanNextDigit;
                    }
                    else if (currentDigit > biggestDigits[INDEX_OF_LAST_DIGIT])
                    {
                        indexOfDigitToDelete = INDEX_OF_LAST_DIGIT;
                    }

                    if (indexOfDigitToDelete == -1)
                    {
                        continue;
                    }

                    DigitsArrayHelper.DeleteAt(biggestDigits, indexOfDigitToDelete);
                    DigitsArrayHelper.Append(biggestDigits, currentDigit);
                }
                else
                {
                    DigitsArrayHelper.Append(biggestDigits, currentDigit);
                }
            }

            long joltage = long.Parse(string.Join("", biggestDigits.Select(digit => digit.ToString())));

            if (joltage > maxJoltage)
            {
                maxJoltage = joltage;
            }
        }

        return maxJoltage;
    }

    public override string ToString()
    {
        return batteries;
    }
}

class DigitsArrayHelper
{
    public static bool IsFull(int[] arr)
    {
        // Based on the assumption that empty slots are filled with 0
        foreach (var digit in arr)
        {
            if (digit == 0)
            {
                return false;
            }
        }

        return true;
    }

    public static void Append(int[] arr, int value)
    {
        int indexToInsertAt = 0;

        // Based on the assumption that empty slots are filled with 0
        while (indexToInsertAt < arr.Length && arr[indexToInsertAt] != 0)
        {
            indexToInsertAt++;
        }

        if (indexToInsertAt >= arr.Length)
        {
            throw new Exception("Cannot append digit to full array");
        }

        arr[indexToInsertAt] = value;
    }

    public static void DeleteAt(int[] arr, int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = 0; // Mark the last element as empty
    }

    public static int FindFirstDigitSmallerThanNext(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            if (arr[i] < arr[i + 1])
            {
                return i;
            }
        }
        return -1;
    }
}
