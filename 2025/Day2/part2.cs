if (args.Length > 0 && args[0] == "--test")
{
    TestRange("11-22", ["11", "22"]);
    TestRange("95-115", ["99", "111"]);
    TestRange("998-1012", ["999", "1010"]);
    TestRange("1188511880-1188511890", ["1188511885"]);
    TestRange("222220-222224", ["222222"]);
    TestRange("1698522-1698528", []);
    TestRange("446443-446449", ["446446"]);
    TestRange("38593856-38593862", ["38593859"]);
    TestRange("565653-565659", ["565656"]);
    TestRange("824824821-824824827", ["824824824"]);
    TestRange("2121212118-2121212124", ["2121212121"]);
    TestPuzzle("Puzzle", "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124", 4174379265);
} 
else
{
    var input = System.IO.File.ReadAllText("input.txt");
    Console.WriteLine($"Password: {Puzzle.FromString(input).GetPassword()}");
}

const string COLOR_RESET = "\x1b[0m";
const string COLOR_GREEN = "\x1b[92m";
const string COLOR_RED = "\x1b[91m";

void TestPuzzle(string testId, string givenInput, long expectedPassword)
{
    long password = Puzzle.FromString(givenInput).GetPassword();

    if (password == expectedPassword)
    {
        Console.WriteLine($"Test {testId} {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test {testId} {COLOR_RED}failed{COLOR_RESET}. Expected {expectedPassword}, got {password}.");
    }
}

void TestRange(string range, IEnumerable<string> expectedIds)
{
    var invalidIds = Range.FromString(range).GetInvalidIds().Select(id => id.ToString());
    if (invalidIds.SequenceEqual(expectedIds))
    {
        Console.WriteLine($"Test {range} {COLOR_GREEN}passed{COLOR_RESET}.");
    }
    else
    {
        Console.WriteLine($"Test {range} {COLOR_RED}failed{COLOR_RESET}. Expected [{string.Join(",", expectedIds)}], got [{string.Join(",", invalidIds)}].");
    }
}

class Puzzle
{
    public List<Range> Ranges { get; private set; } = [];

    public static Puzzle FromString(string puzzleStr)
    {
        var ranges = puzzleStr.Split(',').Select(Range.FromString).ToList();
        return new Puzzle { Ranges = ranges };
    }

    private IEnumerable<Id> GetInvalidIds()
    {
        List<Id> invalidIds = [];

        foreach (var range in Ranges)
        {
            invalidIds.AddRange(range.GetInvalidIds());
        }

        return invalidIds; 
    }

    public long GetPassword()
    {
        return GetInvalidIds().Select(id => long.Parse(id.ToString())).Sum();
    }
}

class Range
{
    private List<Id> ids = [];

    public static Range FromString(string rangeStr)
    {
        var parts = rangeStr.Split('-');
        long start = long.Parse(parts[0]);
        long end = long.Parse(parts[1]);
        var ids = new List<Id>();

        for (long i = start; i <= end; i++)
        {
            ids.Add(new Id(i.ToString()));
        }

        return new Range { ids = ids };
    }

    public IEnumerable<Id> GetInvalidIds()
    {
        List<Id> invalidIds = [];

        foreach (var id in ids)
        {
            if (!id.IsValid())
            {
                invalidIds.Add(id);
            }
        }

        return invalidIds; 
    }
}

class Id(string value)
{
    private string value = value;

    public bool IsValid()
    {
        for (int splitBy = 2; splitBy <= value.Length; splitBy++)
        {
            if (!this.IsSplitable(splitBy))
            {
                continue;
            }
            
            List<string> parts = [];
            int partLength = value.Length / splitBy;

            for (int partIndex = 0; partIndex < splitBy; partIndex++)
            {
                parts.Add(value.Substring(partIndex * partLength, partLength));
            }

            if (parts.All(p => p == parts[0]))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsSplitable(int splitBy)
    {
        return value.Length % splitBy == 0;
    }

    public override string ToString()
    {
        return value;
    }
}

