namespace AdventOfCode2022;

public class Day17 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var vents = input[0].Select((x, i) => x switch
		{
			'<' => new Vent(i, Vector2D.Left),
			'>' => new Vent(i, Vector2D.Right),
			_ => throw new NotSupportedException($"{x}"),
		});

		yield return GetRockTowerHeight(2022, vents).ToString();
		yield return GetRockTowerHeight(1_000_000_000_000, vents).ToString();
	}

	private record Vent(int Id, Vector2D Direction);

	private record Rock(int Id, IReadOnlyList<Vector2D> Locations)
	{
		public Rock Move(Vector2D direction) => this with { Locations = Locations.Select(x => x + direction).ToList() };
	}

	private long GetRockTowerHeight(long totalRockCount, IEnumerable<Vent> vents)
	{
		long rockCount = 0;
		long towerHeight = 0;
		var floor = new HashSet<Vector2D>();
		var snapshots = new Dictionary<(int, int, string), (long, long)>();
		var rockStream = GetInfiniteStream(s_rocks).GetEnumerator();
		var ventStream = GetInfiniteStream(vents).GetEnumerator();

		while (rockStream.MoveNext())
		{
			int maxFloorHeight = -(floor.MinBy(x => x.Y)?.Y ?? 0);
			var rock = rockStream.Current
				.Move(Vector2D.Right * 3)
				.Move(Vector2D.Up * (maxFloorHeight + 4));

			while (ventStream.MoveNext())
			{
				tryMoveRock(ref rock, ventStream.Current.Direction);
				if (!tryMoveRock(ref rock, Vector2D.Down))
					break;
			}

			var snapshot = (rock.Id, ventStream.Current.Id, string.Join("", floor.OrderBy(x => x.X).ThenBy(x => x.Y)));
			if (snapshots.ContainsKey(snapshot))
			{
				var (towerHeightAtSnapshot, rockCountAtSnapshot) = snapshots[snapshot];
				long divResult = Math.DivRem(totalRockCount - rockCountAtSnapshot, rockCount - rockCountAtSnapshot, out long rem);
				if (rem == 0)
					return towerHeight + ((towerHeight - towerHeightAtSnapshot) * (divResult - 1));
			}
			else 
			{
				snapshots.Add(snapshot, (towerHeight, rockCount));
			}
			
			foreach (var location in rock.Locations)
			{
				if (!floor.Add(location))
					throw new InvalidOperationException($"Already added rock location {location}");

				int locationHeight = -location.Y;
				if (locationHeight > maxFloorHeight)
				{
					towerHeight += (locationHeight - maxFloorHeight);
					maxFloorHeight = locationHeight;
				}
			}

			if (++rockCount == totalRockCount)
				break;
			
			var maxFloorHeights = floor.GroupBy(x => x.X).Select(x => -x.MinBy(x => x.Y).Y).ToList();
			if (maxFloorHeights.Count == 7)
			{
				int minFloorHeight = maxFloorHeights.Min();
				floor = floor
					.Where(x => -x.Y > minFloorHeight)
					.Select(x => x.Add(Vector2D.Down * minFloorHeight))
					.ToHashSet();
			}
		}

		return towerHeight;

		bool tryMoveRock(ref Rock rock, Vector2D direction)
		{
			var movedRock = rock.Move(direction);
			foreach (var location in movedRock.Locations)
			{
				if (location.Y == 0 || location.X == 0 || location.X == 8 || floor.Contains(location))
					return false;
			}

			rock = movedRock;
			return true;
		}
	}

	private static IEnumerable<T> GetInfiniteStream<T>(IEnumerable<T> items)
	{
		while (true)
		{
			foreach (var itemWithIndex in items)
				yield return itemWithIndex;
		}
	}

	private static readonly IReadOnlyList<Rock> s_rocks = new[]
	{
		new Rock(0, new Vector2D[] { (0, 0), (1, 0), (2, 0), (3, 0) }),
		new Rock(1, new Vector2D[] { (0, -1), (1, -1), (2, -1), (1, 0), (1, -2) }),
		new Rock(2, new Vector2D[] { (0, 0), (1, 0), (2, 0), (2, -1), (2, -2) }),
		new Rock(3, new Vector2D[] { (0, 0), (0, -1), (0, -2), (0, -3) }),
		new Rock(4, new Vector2D[] { (0, 0), (0, -1), (1, 0), (1, -1) }),
	};
}