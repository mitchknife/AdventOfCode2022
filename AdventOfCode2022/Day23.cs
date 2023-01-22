namespace AdventOfCode2022;

public class Day23 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var elves = input
			.SelectMany((line, iy) => line
				.Select((ch, ix) => (ch, ix))
				.Where(x => x.ch is '#')
				.Select(x => new Vector2D(x.ix, iy)))
			.ToList();
		
		yield return getNumberOfEmptyLocationsAfter10Rounds().ToString();
		yield return getFirstRoundWhereNoElfMoves().ToString();

		int getNumberOfEmptyLocationsAfter10Rounds()
		{
			var model = new Model(elves);
			foreach (int round in Enumerable.Range(1, 10))
				model.ExecuteRound();
			
			return model.GetNumberOfEmptyLocations();
		}

		int getFirstRoundWhereNoElfMoves()
		{
			var model = new Model(elves);
			while (model.ExecuteRound() != 0) { }
			return model.Rounds;
		}
	}

	private class Model
	{
		public Model(IReadOnlyList<Vector2D> elfLocations)
		{
			m_elves = elfLocations.ToHashSet();
			m_moves = new Queue<Direction>(new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right });
		}

		public int Rounds { get; private set; }

		public int ExecuteRound()
		{
			Rounds++;
			var proposedMoves = new Dictionary<Vector2D, Vector2D>();
			foreach (var location in m_elves)
			{
				var validLocations = m_moves
					.Select(dir => (dir, proposed: location.Move(dir)))
					.Where(x => !m_elves.Contains(x.proposed))
					.Where(x => x.dir switch
					{
						Direction.Up or Direction.Down => !m_elves.Contains(x.proposed.Move(Direction.Left)) &&
							!m_elves.Contains(x.proposed.Move(Direction.Right)),
						Direction.Left or Direction.Right => !m_elves.Contains(x.proposed.Move(Direction.Up)) &&
							!m_elves.Contains(x.proposed.Move(Direction.Down)),
						_ => throw new ArgumentOutOfRangeException(),
					})
					.Select(x => x.proposed)
					.ToList();

				if (validLocations.Count is 1 or 2 or 3)
					proposedMoves.Add(location, validLocations.First());
			}

			int numberOfMoves = 0;
			foreach (var (current, proposed) in proposedMoves
				.GroupBy(x => x.Value)
				.Where(x => x.Count() == 1)
				.Select(x => (x.Single().Key, x.Single().Value)))
			{
				m_elves.Remove(current);
				m_elves.Add(proposed);
				numberOfMoves++;
			}

			m_moves.Enqueue(m_moves.Dequeue());
			return numberOfMoves;
		}

		public int GetNumberOfEmptyLocations()
		{
			var (xMin, xMax, yMin, yMax) = m_elves.Aggregate(
				(xMin: int.MaxValue, xMax: int.MinValue, yMin: int.MaxValue, yMax: int.MinValue),
				(ac, loc) => (
					Math.Min(loc.X, ac.xMin),
					Math.Max(loc.X, ac.xMax),
					Math.Min(loc.Y, ac.yMin),
					Math.Max(loc.Y, ac.yMax)
				));

			return (xMax - xMin + 1) * (yMax - yMin + 1) - m_elves.Count;
		}

		private readonly HashSet<Vector2D> m_elves;
		private readonly Queue<Direction> m_moves;
	}
}
