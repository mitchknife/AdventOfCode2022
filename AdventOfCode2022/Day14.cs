namespace AdventOfCode2022;

public class Day14 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var cave = new Cave(input);

		int grainsOfSand = 0;
		while(cave.DropSand((500, 0), floorLevel: null))
			grainsOfSand++;

		yield return grainsOfSand.ToString();

		cave.ClearSand();

		grainsOfSand = 0;
		while(cave.DropSand((500, 0), floorLevel: cave.MaxY + 2))
			grainsOfSand++;

		yield return grainsOfSand.ToString();
	}

	private class Cave
	{
		public Cave(IReadOnlyList<string> input)
		{
			foreach (string line in input)
			{
				var vertexes = line.Split(" -> ")
					.Select(x => x.Split(','))
					.Select(x => new Vector2D(int.Parse(x[0]), int.Parse(x[1])))
					.ToList();
				
				foreach (var (start, end) in vertexes.Zip(vertexes.Skip(1)))
				{
					m_rock.Add(start);
					m_rock.Add(end);

					var diff = end - start;
					var direction =
						diff.Y > 0 ? Vector2D.Down :
						diff.Y < 0 ? Vector2D.Up :
						diff.X > 0 ? Vector2D.Right :
						diff.X < 0 ? Vector2D.Left :
						throw new ArgumentOutOfRangeException();

					var point = start;
					do
					{
						point += direction;
						m_rock.Add(point);
					} while (point != end);
				}
			}

			MaxY = m_rock.Select(x => x.Y).Max();
		}

		public int MaxY { get; }

		public void ClearSand() => m_sand.Clear();

		public bool DropSand(Vector2D from, int? floorLevel)
		{
			if (floorLevel is null && from.Y > MaxY)
				return false;

			if (IsBlocked(from, floorLevel))
				return false;

			var next = from + Vector2D.Down;
			if (IsBlocked(next, floorLevel))
			{
				next = from + Vector2D.Down + Vector2D.Left;
				if (IsBlocked(next, floorLevel))
				{
					next = from + Vector2D.Down + Vector2D.Right;
					if (IsBlocked(next, floorLevel))
					{
						m_sand.Add(from);
						return true;
					}
				}
			}
			
			return DropSand(next, floorLevel);
		}

		private bool IsBlocked(Vector2D point, int? floorLevel)
			=> point.Y == floorLevel || m_rock.Contains(point) || m_sand.Contains(point);

		private readonly HashSet<Vector2D> m_rock = new HashSet<Vector2D>();
		private readonly HashSet<Vector2D> m_sand = new HashSet<Vector2D>();
	}
}
