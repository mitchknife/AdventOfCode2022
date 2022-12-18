namespace AdventOfCode2022;

public record Vector(int X, int Y)
{
    public Vector Add(Vector vector) => new Vector(X + vector.X, Y + vector.Y);
    public Vector Subtract(Vector vector) => new Vector(X - vector.X, Y - vector.Y);
    public override string ToString() => $"({X}, {Y})";

    public static readonly Vector Zero = new(0, 0);
    public static readonly Vector Up = new(0, -1);
    public static readonly Vector Down = new(0, 1);
    public static readonly Vector Left = new(-1, 0);
    public static readonly Vector Right = new(1, 0);
}