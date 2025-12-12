using System;
using System.IO;
using System.Text.RegularExpressions;

if (args.Length > 0 && args[0] == "--test")
{
    TestRangeFromString("5-10", 5, 10);
    TestRangeFromString("0-0", 0, 0);
    TestRangeFromString("100-200", 100, 200);
    TestRangeFromString("42-42", 42, 42);
    TestRangeFromString("1-999999", 1, 999999);
    TestRangeFromString("339893944525735-341573247957386", 339893944525735, 341573247957386);
    TestRangeContains(new Range(5, 10), 7, true);
    TestRangeContains(new Range(5, 10), 5, true);
    TestRangeContains(new Range(5, 10), 10, true);
    TestRangeContains(new Range(5, 10), 4, false);
    TestRangeContains(new Range(5, 10), 11, false);
    TestRangeContains(new Range(339893944525735, 341573247957386), 340000000000000, true);
    TestRangeContains(new Range(339893944525735, 341573247957386), 339000000000000, false);
    TestPuzzle(System.IO.File.ReadAllText("test-input.txt"), 3);
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

void TestRangeFromString(string givenInput, long expectedStart, long expectedEnd)
{
    var range = Range.FromString(givenInput);
    if (range.Start != expectedStart || range.End != expectedEnd)
    {
        Console.WriteLine($"Range.FromString({givenInput}) {COLOR_RED}failed{COLOR_RESET}. Expected Start={expectedStart}, End={expectedEnd}, got Start={range.Start}, End={range.End}");
    }
    else
    {
        Console.WriteLine($"Range.FromString({givenInput}) {COLOR_GREEN}passed{COLOR_RESET}.");
    }
}

void TestRangeContains(Range range, long value, bool expectedContains)
{
    bool contains = range.Contains(value);
    if (contains != expectedContains)
    {
        Console.WriteLine($"Range.Contains({value}) {COLOR_RED}failed{COLOR_RESET}. Expected {expectedContains}, got {contains}.");
    }
    else
    {
        Console.WriteLine($"Range.Contains({value}) {COLOR_GREEN}passed{COLOR_RESET}.");
    }
}

class Puzzle
{
    private List<Range> freshIngredientRanges = [];
    private List<long> availableIngredients = [];

    public static Puzzle FromString(string input)
    {
        string[] inputParts = Regex.Split(input.Trim(), @"\r?\n\s*\r?\n");

        string rangesStr = inputParts.Length > 0 ? inputParts[0] : throw new Exception("Invalid input");
        string ingresdientsStr = inputParts.Length > 1 ? inputParts[1] : throw new Exception("Invalid input");

        return new Puzzle
        { 
            freshIngredientRanges = rangesStr.Split('\n').Select(r => Range.FromString(r)).ToList(),
            availableIngredients = ingresdientsStr.Split('\n').Select(i => long.Parse(i)).ToList()
        };
    }

    public long GetPassword()
    {
        long nbFreshIngredients = 0;
        foreach (long ingredient in availableIngredients)
        {
            bool isFresh = false;
            foreach (var range in freshIngredientRanges)
            {
                if (range.Contains(ingredient))
                {
                    isFresh = true;
                    break;
                }
            }
            if (isFresh)
            {
                nbFreshIngredients++;
            }
        }
        return nbFreshIngredients;
    }
}

class Range(long start, long end)
{
    public long Start { get; private set; } = start;
    public long End { get; private set; } = end;

    public static Range FromString(string input)
    {
        var parts = input.Split('-');
        if (parts.Length != 2)
        {
            throw new Exception("Invalid range string");
        }
        return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
    }

    public bool Contains(long value)
    {
        return value >= Start && value <= End;
    }

    public override string ToString()
    {
        return $"{Start}-{End}";
    }
}