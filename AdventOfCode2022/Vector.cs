namespace AdventOfCode2022;

public record Vector
{
	public Vector(int x, int y)
	{
		X = x;
		Y = y;
	}

	public Vector(int x, int y, int z)
		: this(x, y)
	{
		m_z = z;
	}

	public int X { get; init; }
	public int Y { get; init; }
	public int Z { get => m_z.Value; init => m_z = value; }

	public Vector Add(Vector vector)
	{
		VerifyDimension(vector);
		return m_z is null ? new Vector(X + vector.X, Y + vector.Y) : new Vector(X + vector.X, Y + vector.Y, Z + vector.Z);
	}

	public Vector Add(int num) => m_z is null ? Add(new Vector(num, num)) : Add(new Vector(num, num, num));

	public Vector Subtract(Vector vector)
	{
		VerifyDimension(vector);
		return m_z is null ? new Vector(X - vector.X, Y - vector.Y) : new Vector(X - vector.X, Y - vector.Y, Z - vector.Z);
	}

	public Vector Subtract(int num) => m_z is null ? Subtract(new Vector(num, num)) : Subtract(new Vector(num, num, num));

	public Vector Multiply(Vector vector)
	{
		VerifyDimension(vector);
		return m_z is null ? new Vector(X * vector.X, Y * vector.Y) : new Vector(X * vector.X, Y * vector.Y, Z * vector.Z);
	}

	public Vector Multiply(int num) => m_z is null ? Multiply(new Vector(num, num)) : Multiply(new Vector(num, num, num));

	public int Distance(Vector vector)
	{
		var diff = vector.Subtract(this);
		int sum = Math.Abs(diff.X) + Math.Abs(diff.Y);
		if (m_z is not null)
			sum += Math.Abs(diff.Z);
		return sum;
	}

	public static Vector operator +(Vector a, Vector b) => a.Add(b);
	public static Vector operator +(Vector a, int b) => a.Add(b);
	public static Vector operator -(Vector a, Vector b) => a.Subtract(b);
	public static Vector operator -(Vector a, int b) => a.Subtract(b);
	public static Vector operator *(Vector a, Vector b) => a.Multiply(b);
	public static Vector operator *(Vector a, int b) => a.Multiply(b);

	public static implicit operator Vector(ValueTuple<int, int, int> tuple) => new Vector(tuple.Item1, tuple.Item2, tuple.Item3);
	public static implicit operator Vector(ValueTuple<int, int> tuple) => new Vector(tuple.Item1, tuple.Item2);

	public void Deconstruct(out int x, out int y, out int z)
	{
		x = X;
		y = Y;
		z = Z;
	}

	public override string ToString() => m_z is null ? $"({X}, {Y})" : $"({X}, {Y}, {Z})";

	public static readonly Vector Zero = new(0, 0);
	public static readonly Vector Up = new(0, -1);
	public static readonly Vector Down = new(0, 1);
	public static readonly Vector Left = new(-1, 0);
	public static readonly Vector Right = new(1, 0);

	private void VerifyDimension(Vector v)
	{
		if (m_z.HasValue != v.m_z.HasValue)
			throw new InvalidOperationException("Cannot compare vectors of different dimensions");
	}

	private int? m_z;
}