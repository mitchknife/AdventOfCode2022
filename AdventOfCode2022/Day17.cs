namespace AdventOfCode2022;

public class Day17 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var vents = input[0].Select((x, i) => x switch
		{
			'<' => new Vent(i, Vector.Left),
			'>' => new Vent(i, Vector.Right),
			_ => throw new NotSupportedException($"{x}"),
		});

		yield return GetRockTowerHeight(2022, vents).ToString();
		yield return GetRockTowerHeight(1_000_000_000_000, vents).ToString();
	}

	private record Vent(int Id, Vector Direction);

	private record Rock(int Id, IReadOnlyList<Vector> Locations)
	{
		public Rock Move(Vector direction) => this with { Locations = Locations.Select(x => x.Add(direction)).ToList() };
	}

	private long GetRockTowerHeight(long totalRockCount, IEnumerable<Vent> vents)
	{
		long rockCount = 0;
		long towerHeight = 0;
		var floor = new HashSet<Vector>();
		var snapshots = new Dictionary<(int, int, string), (long, long)>();
		var rockStream = GetInfiniteStream(s_rocks).GetEnumerator();
		var ventStream = GetInfiniteStream(vents).GetEnumerator();

		while (rockStream.MoveNext())
		{
			int maxFloorHeight = -(floor.MinBy(x => x.Y)?.Y ?? 0);
			var rock = rockStream.Current
				.Move(Vector.Right.Multiply(3))
				.Move(Vector.Up.Multiply(maxFloorHeight + 4));

			while (ventStream.MoveNext())
			{
				tryMoveRock(ref rock, ventStream.Current.Direction);
				if (!tryMoveRock(ref rock, Vector.Down))
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
					.Select(x => x.Add(Vector.Down.Multiply(minFloorHeight)))
					.ToHashSet();
			}
		}

		return towerHeight;

		bool tryMoveRock(ref Rock rock, Vector direction)
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
		new Rock(0, new Vector[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) }),
		new Rock(1, new Vector[] { new(0, -1), new(1, -1), new(2, -1), new(1, 0), new(1, -2) }),
		new Rock(2, new Vector[] { new(0, 0), new(1, 0), new(2, 0), new(2, -1), new(2, -2) }),
		new Rock(3, new Vector[] { new(0, 0), new(0, -1), new(0, -2), new(0, -3) }),
		new Rock(4, new Vector[] { new(0, 0), new(0, -1), new(1, 0), new(1, -1) }),
	};
}