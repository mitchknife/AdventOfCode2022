namespace AdventOfCode2022;

public record Vector3D(int X, int Y, int Z)
{
	public Vector3D Add(Vector3D vector) => new Vector3D(X + vector.X, Y + vector.Y, Z + vector.Z);

	public Vector3D Add(int num) => Add(new Vector3D(num, num, num));

	public Vector3D Subtract(Vector3D vector) => new Vector3D(X - vector.X, Y - vector.Y, Z - vector.Z);

	public Vector3D Subtract(int num) => Subtract(new Vector3D(num, num, num));

	public Vector3D Multiply(Vector3D vector) => new Vector3D(X * vector.X, Y * vector.Y, Z * vector.Z);

	public Vector3D Multiply(int num) => Multiply(new Vector3D(num, num, num));

	public int Distance(Vector3D vector)
	{
		var diff = vector.Subtract(this);
		return Math.Abs(diff.X) + Math.Abs(diff.Y) + Math.Abs(diff.Z);
	}

	public static Vector3D operator +(Vector3D a, Vector3D b) => a.Add(b);
	public static Vector3D operator +(Vector3D a, int b) => a.Add(b);
	public static Vector3D operator -(Vector3D a, Vector3D b) => a.Subtract(b);
	public static Vector3D operator -(Vector3D a, int b) => a.Subtract(b);
	public static Vector3D operator *(Vector3D a, Vector3D b) => a.Multiply(b);
	public static Vector3D operator *(Vector3D a, int b) => a.Multiply(b);

	public static implicit operator Vector3D(ValueTuple<int, int, int> tuple) => new Vector3D(tuple.Item1, tuple.Item2, tuple.Item3);

	public override string ToString() => $"({X}, {Y}, {Z})";

	public static readonly Vector3D Zero = (0, 0, 0);
	public static readonly Vector3D Min = (int.MinValue, int.MinValue, int.MinValue);
	public static readonly Vector3D Max = (int.MaxValue, int.MaxValue, int.MaxValue);
}