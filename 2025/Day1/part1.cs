const int DIAL_START_POSITION = 50;

var dial = new Dial(DIAL_START_POSITION);

// TEST

// const string GIVEN_TEST_INPUT = "L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82";
// const int EXPECTED_TEST_PASSWORD = 3;

// var rotations = GIVEN_TEST_INPUT
//     .Split('\n', StringSplitOptions.RemoveEmptyEntries)
//     .Select(Rotation.FromString); 

// dial.Rotate(rotations);

// if (dial.NumberOTimesTheDialPointedToZero == EXPECTED_TEST_PASSWORD)
// {
//     Console.WriteLine("Test passed.");
// }
// else
// {
//     Console.WriteLine($"Test failed. Expected {EXPECTED_TEST_PASSWORD}, got {dial.NumberOTimesTheDialPointedToZero}");
// }

// TEST END

var rotations = File.ReadLines("input.txt")
    .Where(line => !string.IsNullOrWhiteSpace(line))
    .Select(Rotation.FromString);

dial.Rotate(rotations);

Console.WriteLine($"Password: {dial.NumberOTimesTheDialPointedToZero}.");


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
        Position = rotation.Apply(Position);

        if (Position == 0)
        {
            NumberOTimesTheDialPointedToZero++;
        }
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

    public int Apply(int currentPosition)
    {
        return direction.IsLeft() ? 
            (currentPosition - distance + 100) % 100 : 
            (currentPosition + distance) % 100;
    }
}

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