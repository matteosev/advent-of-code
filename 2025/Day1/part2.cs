if (args.Length > 0 && args[0] == "--test")
{
    Test("1", "L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82", 6);
    Test("2", "R1000", 10);
    Test("3", "L1000", 10);
    Test("4", "R50", 1);
    Test("5", "L50", 1);
} 
else
{
    const int DIAL_START_POSITION = 50;

    var rotations = File.ReadLines("input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(Rotation.FromString);

    var dial = new Dial(DIAL_START_POSITION);
    dial.Rotate(rotations);

    Console.WriteLine($"Password: {dial.NumberOTimesTheDialPointedToZero}.");
}

void Test(string testId, string givenInput, int expectedPassword)
{
    var dial = new Dial(50);

    var rotations = givenInput
        .Split('\n')
        .Select(Rotation.FromString); 

    dial.Rotate(rotations);

    if (dial.NumberOTimesTheDialPointedToZero == expectedPassword)
    {
        Console.WriteLine($"Test {testId} passed.");
    }
    else
    {
        Console.WriteLine($"Test {testId} failed. Expected {expectedPassword}, got {dial.NumberOTimesTheDialPointedToZero}");
    }
}

class Dial(int startPosition)
{
    public int Position { get; private set; } = startPosition;
    public int NumberOTimesTheDialPointedToZero { get; private set; } = 0;

    public void Rotate(IEnumerable<Rotation> rotations)
    {
        foreach (var rotation in rotations)
        {
            Rotate(rotation);
        }
    }

    public void Rotate(Rotation rotation)
    {
        var rotationResult = rotation.Apply(Position);
        Position = rotationResult.NewPosition;
        NumberOTimesTheDialPointedToZero += rotationResult.NumberOTimesTheDialPointedToZero;
    }
}

class Rotation
{
    private readonly Direction direction;
    private readonly int distance;

    private Rotation(Direction direction, int distance)
    {
        this.direction = direction;
        this.distance = distance;
    }

    public static Rotation FromString(string input)
    {
        var direction = Direction.FromChar(input[0]);
        int distance = int.Parse(input[1..]);
        return new Rotation(direction, distance);
    }

    public RotationResult Apply(int currentPosition)
    {
        if (currentPosition < 0)
        {
            throw new ArgumentException($"Current position cannot be negative: {currentPosition}");
        }

        int newPosition = direction.IsLeft() ? 
            Math.Abs(currentPosition - distance + 100) % 100 : 
            (currentPosition + distance) % 100;

        int numberOTimesTheDialPointedToZero = direction.IsLeft() ? 
            CountNumberOTimesTheDialPointedToZeroToTheLeft(currentPosition, distance) : 
            CountNumberOTimesTheDialPointedToZeroToTheRight(currentPosition, distance);

        Console.WriteLine($"New position: {newPosition}, Number of times dial pointed to zero: {numberOTimesTheDialPointedToZero}, Direction: {(direction.IsLeft() ? "L" : "R")}, Distance: {distance}");

        return new(newPosition, numberOTimesTheDialPointedToZero);
    }

    private int CountNumberOTimesTheDialPointedToZeroToTheLeft(int currentPosition, int distance)
    {
        int positionMinusDistance = currentPosition - distance;
        
        // Console.WriteLine($"Position minus distance: {positionMinusDistance}");

        if (positionMinusDistance > 0)
        {
            return 0;
        }
        else if (positionMinusDistance == 0)
        {
            return 1;
        }
        
        // Console.WriteLine($"Absolute position minus distance: {Math.Abs(positionMinusDistance) / 100}");

        return Math.Abs(positionMinusDistance) / 100 + (currentPosition == 0 ? 0 : 1);
    }

    private int CountNumberOTimesTheDialPointedToZeroToTheRight(int currentPosition, int distance)
    {
        // Console.Write($"Current position plus distance: {currentPosition + distance} -> ");
        return (currentPosition + distance) / 100;
    }
}

record RotationResult(int NewPosition, int NumberOTimesTheDialPointedToZero);

abstract class Direction
{
    public static Direction FromChar(char input)
    {
        return input switch
        {
            'L' => new LeftDirection(),
            'R' => new RightDirection(),
            _ => throw new ArgumentException("Invalid direction character"),
        };
    }

    public abstract bool IsLeft();
    public abstract bool IsRight();
}

class RightDirection : Direction
{
    public override bool IsLeft() => false;
    public override bool IsRight() => true;
}

class LeftDirection : Direction
{
    public override bool IsLeft() => true;
    public override bool IsRight() => false;
}