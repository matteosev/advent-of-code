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
    TestRangeIsOverlapping(new Range(5, 10), new Range(7, 12), true);
    TestRangeIsOverlapping(new Range(5, 10), new Range(1, 5), true);
    TestRangeIsOverlapping(new Range(5, 10), new Range(10, 15), true);
    TestRangeIsOverlapping(new Range(5, 10), new Range(1, 4), false);
    TestRangeIsOverlapping(new Range(5, 10), new Range(11, 15), false);
    TestRangeIsOverlapping(new Range(339893944525735, 341573247957386), new Range(340000000000000, 342000000000000), true);
    TestRangeIsOverlapping(new Range(339893944525735, 341573247957386), new Range(338000000000000, 339000000000000), false);
    TestPuzzle(System.IO.File.ReadAllText("test-input.txt"), 14);
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

void TestRangeIsOverlapping(Range range1, Range range2, bool expectedOverlapping)
{
    bool isOverlapping = range1.IsOverlapping(range2);
    if (isOverlapping != expectedOverlapping)
    {
        Console.WriteLine($"Range.IsOverlapping({range1}, {range2}) {COLOR_RED}failed{COLOR_RESET}. Expected {expectedOverlapping}, got {isOverlapping}.");
    }
    else
    {
        Console.WriteLine($"Range.IsOverlapping({range1}, {range2}) {COLOR_GREEN}passed{COLOR_RESET}.");
    }
}

class Puzzle
{
    private List<Range> freshIngredientRanges = [];

    public static Puzzle FromString(string input)
    {
        string[] inputParts = Regex.Split(input.Trim(), @"\r?\n\s*\r?\n");

        string rangesStr = inputParts.Length > 0 ? inputParts[0] : throw new Exception("Invalid input");

        var p = new Puzzle
        { 
            freshIngredientRanges = rangesStr.Split('\n').Select(r => Range.FromString(r)).ToList()
        };

        p.MergeOverlappingRanges();

        return p;
    }

    private void MergeOverlappingRanges()
    {
        for (int i = 0; i < freshIngredientRanges.Count - 1; i++)  // Range I'm trying to merge others with
        {
            bool changedCurrentRange = false;
            for (int j = i + 1; j < freshIngredientRanges.Count; j++)    // Ranges to compare against
            {
                if (freshIngredientRanges[i].IsOverlapping(freshIngredientRanges[j]))
                {
                    long newStart = Math.Min(freshIngredientRanges[i].Start, freshIngredientRanges[j].Start);
                    long newEnd = Math.Max(freshIngredientRanges[i].End, freshIngredientRanges[j].End);
                    var mergedRange = new Range(newStart, newEnd);

                    freshIngredientRanges[i] = mergedRange;
                    freshIngredientRanges.RemoveAt(j);
                    changedCurrentRange = true;
                    j--;    // Adjust index after removal
                }
            }
            if (changedCurrentRange)
            {
                i--;    // Re-evaluate the current range after merging
            }
        }
    }

    public long GetPassword()
    {
        long nbFreshIngredients = 0;
        foreach (var range in freshIngredientRanges)
        {
            nbFreshIngredients += range.End - range.Start + 1;
        }
        return nbFreshIngredients;
    }

    public override string ToString()
    {
        return string.Join("\n", freshIngredientRanges.Select(r => r.ToString()));
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

    public bool IsOverlapping(Range other)
    {
        return !(other.End < Start || other.Start > End);
    }

    public override string ToString()
    {
        return $"{Start}-{End}";
    }
}
