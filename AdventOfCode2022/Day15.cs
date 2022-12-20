using System.Numerics;
namespace AdventOfCode2022;

public class Day15 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var sensors = new List<Sensor>();
		foreach (string line in input.Skip(2))
		{
			var tokens = line.Split(' ');

			sensors.Add(new Sensor(
				Location: new Vector(
					int.Parse(tokens[2].Split('=')[1].Trim(',')),
					int.Parse(tokens[3].Split('=')[1].Trim(':'))),
				Beacon: new Vector(
					int.Parse(tokens[8].Split('=')[1].Trim(',')),
					int.Parse(tokens[9].Split('=')[1]))));
		}

		int part1LineToCheck = int.Parse(input[0]);
		var part1BeaconXLocations = sensors
			.Select(x => x.Beacon)
			.Where(x => x.Y == part1LineToCheck)
			.Select(x => x.X)
			.ToHashSet();

		yield return sensors
			.Select(x => x.GetAffectedXRange(part1LineToCheck))
			.Where(x => x is not null)
			.SelectMany(x => Enumerable.Range(x.From, x.To - x.From + 1))
			.Distinct()
			.Where(x => !part1BeaconXLocations.Contains(x))
			.Select(x => new Vector(x, part1LineToCheck))
			.Count()
			.ToString();

		int part2Max = int.Parse(input[1]);
		for (int y = part2Max; y >= 0; y--)
		{
			var sensorRanges = sensors
				.Select(x => x.GetAffectedXRange(y))
				.Where(x => x is not null)
				.ToList();
			
			var combinedRanges = CombineRanges(0, part2Max, sensorRanges);
			if (combinedRanges.Count == 2)
			{
				int x = combinedRanges[0].To + 1;
				yield return (new BigInteger(4000000) * new BigInteger(x) + new BigInteger(y)).ToString();
				yield break;
			}
		}
	}

	private static IReadOnlyList<Range> CombineRanges(int min, int max, IReadOnlyList<Range> ranges)
	{
		var combinedRanges = new List<Range>();
		foreach (var range in ranges.Select(x => new Range(Math.Max(min, x.From),Math.Min(max, x.To))).OrderBy(x => x.From))
		{
			var last = combinedRanges.LastOrDefault();
			if (last is null)
			{
				combinedRanges.Add(range);
			}
			if (last is not null)
			{
				if (range.From <= last.To + 1 && range.To > last.To)
					combinedRanges[combinedRanges.Count - 1] = last with { To = range.To };
				else if (range.From > last.To + 1)
					combinedRanges.Add(range);
			}
		}

		return combinedRanges;
	}

	private record Range(int From, int To);

	private record Sensor(Vector Location, Vector Beacon)
	{
		public int DistanceFromBeacon = Location.Distance(Beacon);

		public Range GetAffectedXRange(int y)
		{
			int diffY = Math.Abs(y - Location.Y);
			if (diffY > DistanceFromBeacon)
				return null;

			int diffX = DistanceFromBeacon - diffY;
			return new Range(Location.X - diffX, Location.X + diffX);
		}
	}
}
