namespace AdventOfCode2022;

public record Vector2D(int X, int Y)
{
	public Vector2D Add(Vector2D vector) => new Vector2D(X + vector.X, Y + vector.Y);
	public Vector2D Add(int num) => Add((num, num));
	public Vector2D Subtract(Vector2D vector) => new Vector2D(X - vector.X, Y - vector.Y);
	public Vector2D Subtract(int num) => Subtract((num, num));
	public Vector2D Multiply(Vector2D vector) => new Vector2D(X * vector.X, Y * vector.Y);
	public Vector2D Multiply(int num) => Multiply(new Vector2D(num, num));

	public int Distance(Vector2D vector)
	{
		var diff = vector.Subtract(this);
		return Math.Abs(diff.X) + Math.Abs(diff.Y);
	}

	public static Vector2D operator +(Vector2D a, Vector2D b) => a.Add(b);
	public static Vector2D operator +(Vector2D a, int b) => a.Add(b);
	public static Vector2D operator -(Vector2D a, Vector2D b) => a.Subtract(b);
	public static Vector2D operator -(Vector2D a, int b) => a.Subtract(b);
	public static Vector2D operator *(Vector2D a, Vector2D b) => a.Multiply(b);
	public static Vector2D operator *(Vector2D a, int b) => a.Multiply(b);
	public static implicit operator Vector2D(ValueTuple<int, int> tuple) => new Vector2D(tuple.Item1, tuple.Item2);

	public override string ToString() => $"({X}, {Y})";

	public static readonly Vector2D Zero = new(0, 0);
	public static readonly Vector2D Up = new(0, -1);
	public static readonly Vector2D Down = new(0, 1);
	public static readonly Vector2D Left = new(-1, 0);
	public static readonly Vector2D Right = new(1, 0);
}