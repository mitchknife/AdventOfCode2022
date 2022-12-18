namespace AdventOfCode2022;

public class Day14 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var cave = new Cave(input);

		int grainsOfSand = 0;
		while(cave.DropSand(new(500, 0), floorLevel: null))
			grainsOfSand++;

		yield return grainsOfSand.ToString();

		cave.ClearSand();

		grainsOfSand = 0;
		while(cave.DropSand(new(500, 0), floorLevel: cave.MaxY + 2))
			grainsOfSand++;

		yield return grainsOfSand.ToString();
	}

	private record Vector(int X, int Y)
	{
		public Vector Add(Vector vector) => new Vector(X + vector.X, Y + vector.Y);
		public Vector Subtract(Vector vector) => new Vector(X - vector.X, Y - vector.Y);
		public override string ToString() => $"({X}, {Y})";

		public static readonly Vector Up = new(0, -1);
		public static readonly Vector Down = new(0, 1);
		public static readonly Vector Left = new(-1, 0);
		public static readonly Vector Right = new(1, 0);
	}

	private class Cave
	{
		public Cave(IReadOnlyList<string> input)
		{
			foreach (string line in input)
			{
				var vertexes = line.Split(" -> ")
					.Select(x => x.Split(','))
					.Select(x => new Vector(int.Parse(x[0]), int.Parse(x[1])))
					.ToList();
				
				foreach (var (start, end) in vertexes.Zip(vertexes.Skip(1)))
				{
					m_rock.Add(start);
					m_rock.Add(end);

					var diff = end.Subtract(start);
					var direction =
						diff.Y > 0 ? Vector.Down :
						diff.Y < 0 ? Vector.Up :
						diff.X > 0 ? Vector.Right :
						diff.X < 0 ? Vector.Left :
						throw new ArgumentOutOfRangeException();

					var point = start;
					do
					{
						point = point.Add(direction);
						m_rock.Add(point);
					} while (point != end);
				}
			}

			MaxY = m_rock.Select(x => x.Y).Max();
		}

		public int MaxY { get; }

		public void ClearSand() => m_sand.Clear();

		public bool DropSand(Vector from, int? floorLevel)
		{
			if (floorLevel is null && from.Y > MaxY)
				return false;

			if (IsBlocked(from, floorLevel))
				return false;

			var next = from.Add(Vector.Down);
			if (IsBlocked(next, floorLevel))
			{
				next = from.Add(Vector.Down).Add(Vector.Left);
				if (IsBlocked(next, floorLevel))
				{
					next = from.Add(Vector.Down).Add(Vector.Right);
					if (IsBlocked(next, floorLevel))
					{
						m_sand.Add(from);
						return true;
					}
				}
			}
			
			return DropSand(next, floorLevel);
		}

		private bool IsBlocked(Vector point, int? floorLevel)
			=> point.Y == floorLevel || m_rock.Contains(point) || m_sand.Contains(point);

		private readonly HashSet<Vector> m_rock = new HashSet<Vector>();
		private readonly HashSet<Vector> m_sand = new HashSet<Vector>();
	}
}
