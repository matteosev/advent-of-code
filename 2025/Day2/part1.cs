if (args.Length > 0 && args[0] == "--test")
{
    TestRange("11-22", ["11", "22"]);
    TestRange("95-115", ["99"]);
    TestRange("998-1012", ["1010"]);
    TestRange("1188511880-1188511890", ["1188511885"]);
    TestRange("222220-222224", ["222222"]);
    TestRange("1698522-1698528", []);
    TestRange("446443-446449", ["446446"]);
    TestRange("38593856-38593862", ["38593859"]);
    TestPuzzle("Puzzle", "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124", 1227775554);
} 
else
{
    var input = System.IO.File.ReadAllText("input.txt");
    Console.WriteLine($"Password: {Puzzle.FromString(input).GetPassword()}");
}

void TestPuzzle(string testId, string givenInput, int expectedPassword)
{
    long password = Puzzle.FromString(givenInput).GetPassword();

    if (password == expectedPassword)
    {
        Console.WriteLine($"Test {testId} passed.");
    }
    else
    {
        Console.WriteLine($"Test {testId} failed. Expected {expectedPassword}, got {password}.");
    }
}

void TestRange(string range, IEnumerable<string> expectedIds)
{
    var invalidIds = Range.FromString(range).GetInvalidIds().Select(id => id.ToString());
    if (invalidIds.SequenceEqual(expectedIds))
    {
        Console.WriteLine($"Test {range} passed.");
    }
    else
    {
        Console.WriteLine($"Test {range} failed, Expected {string.Join(",", expectedIds)}, got {string.Join(",", invalidIds)}.");
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
        if (value.Length == 1 || value.Length % 2 == 1)
        {
            return true;
        }

        string[] parts = [value.Substring(0, value.Length / 2), value.Substring(value.Length / 2)];

        if (parts[0] == parts[1])
        {
            return false;
        }

        return true;
    }

    public override string ToString()
    {
        return value;
    }
}
