namespace AdventOfCode2022;

public class Day24 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var model = Model.Parse(input);
		var entrance = model.GetEntrance();
		var exit = model.GetExit();

		int bestFinishTime = model.CalculateBestFinishTime(entrance, exit, 0);
		yield return bestFinishTime.ToString();

		bestFinishTime = model.CalculateBestFinishTime(exit, entrance, bestFinishTime);
		bestFinishTime = model.CalculateBestFinishTime(entrance, exit, bestFinishTime);
		yield return bestFinishTime.ToString();
	}

	private sealed class Model
	{
		public static Model Parse(IReadOnlyList<string> input)
		{
			var tiles = new HashSet<Vector2D>();
			var storms = new List<(Vector2D, Direction)>();

			foreach (var (line, y) in input.WithIndexes())
			{
				foreach (var (ch, x) in line.WithIndexes().Where(x => x.Item is not '#'))
				{
					tiles.Add((x, y));
					if (ch is '^' or '>' or 'v' or '<')
					{
						storms.Add(((x, y), ch switch
						{
							'^' => Direction.Up,
							'>' => Direction.Right,
							'v' => Direction.Down,
							_ => Direction.Left,
						}));
					}
				}
			}
			
			return new Model(tiles, generateAllAvailableLocations());

			IReadOnlyList<HashSet<Vector2D>> generateAllAvailableLocations()
			{
				var min = Vector2D.Zero;
				var max = new Vector2D(input[0].Length - 1, input.Count - 1);
				var allAvailableLocations = new List<HashSet<Vector2D>>();
				var stormsCache = new HashSet<string>();

				while (stormsCache.Add(string.Join("", storms)))
				{
					allAvailableLocations.Add(tiles.Except(storms.Select(x => x.Item1)).ToHashSet());
					storms = storms.Select(moveStorm).ToList();
				}

				return allAvailableLocations;

				(Vector2D, Direction) moveStorm((Vector2D, Direction) storm)
				{
					var (location, direction) = storm;
					var nextLocation = location.Move(direction);
					if (!tiles.Contains(nextLocation))
					{
						nextLocation = direction switch
						{
							Direction.Up => location with { Y = max.Y - 1 },
							Direction.Right => location with { X = min.X + 1 },
							Direction.Down => location with { Y = min.Y + 1 },
							Direction.Left => location with { X = max.X - 1 },
							_ => throw new ArgumentOutOfRangeException(),
						};
					}

					return (nextLocation, direction);
				}
			}
		}

		public Vector2D GetEntrance() => m_tiles.OrderBy(x => x.X).ThenBy(x => x.Y).First();
		public Vector2D GetExit() => m_tiles.OrderByDescending(x => x.X).ThenByDescending(x => x.Y).First();

		public int CalculateBestFinishTime(Vector2D start, Vector2D finish, int startMinutes)
		{
			var bestTimes = new Dictionary<(int, Vector2D), int?>();
			int bestFinishTime = int.MaxValue;
			calculateBestFinishTime(start, startMinutes);
			return bestFinishTime;

			void calculateBestFinishTime(Vector2D location, int minutes)
			{
				if (location == finish)
				{
					bestFinishTime = Math.Min(bestFinishTime, minutes);
					return;
				}

				var bestTimesKey = (minutes % m_allAvailableLocations.Count, location);
				if (bestTimes.GetValueOrDefault(bestTimesKey) <= minutes)
					return;

				bestTimes[bestTimesKey] = minutes;

				if (minutes + location.Distance(finish) >= bestFinishTime)
					return;

				var availableLocations = m_allAvailableLocations[++minutes % m_allAvailableLocations.Count];
				foreach (var move in getMoves(location, finish).Where(availableLocations.Contains))
					calculateBestFinishTime(move, minutes);
			}

			static IEnumerable<Vector2D> getMoves(Vector2D from, Vector2D to)
			{
				var diff = to - from;
				var directions = diff.X > 0 || diff.Y > 0 ?
					new[] { Direction.Right, Direction.Down, (Direction?) null, Direction.Up, Direction.Left } :
					new[] { Direction.Left, Direction.Up, (Direction?) null, Direction.Down, Direction.Right };

				return directions.Select(d => d is null ? from : from.Move(d.Value));
			}
		}

		private Model(IReadOnlyCollection<Vector2D> tiles, IReadOnlyList<HashSet<Vector2D>> allAvailableLocations)
		{
			m_tiles = tiles;
			m_allAvailableLocations = allAvailableLocations;
		}

		private readonly IReadOnlyCollection<Vector2D> m_tiles;
		private readonly IReadOnlyList<HashSet<Vector2D>> m_allAvailableLocations;
	}
}
