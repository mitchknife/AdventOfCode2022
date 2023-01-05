namespace AdventOfCode2022;

public class Day19 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var blueprints = input
			.Select(line => line.Split(' ')
				.Select(x => x.TrimEnd(':'))
				.Where(x => int.TryParse(x, out var _))
				.Select(int.Parse)
				.ToList())
			.Select(ints => new Blueprint(ints[0], new(
				(Resource.Ore, new ResourceValues<int>(
					(Resource.Ore, ints[1]))),
				(Resource.Clay, new ResourceValues<int>(
					(Resource.Ore, ints[2]))),
				(Resource.Obsidian, new ResourceValues<int>(
					(Resource.Ore, ints[3]),
					(Resource.Clay, ints[4]))),
				(Resource.Geode, new ResourceValues<int>(
					(Resource.Ore, ints[5]),
					(Resource.Obsidian, ints[6])))
			)))
			.ToList();

		
		var blueprint = blueprints[1];
		yield return GetBlueprintQualityLevel(blueprint, 24).ToString();
	}

	private int GetBlueprintQualityLevel(Blueprint blueprint, int totalMinutes)
	{
		var state = new State(
			minutes: 0,
			robots: new ResourceValues<int>(
				(Resource.Ore, 1),
				(Resource.Clay, 0),
				(Resource.Obsidian, 0),
				(Resource.Geode, 0)),
			resources: new ResourceValues<int>(
				(Resource.Ore, 0),
				(Resource.Clay, 0),
				(Resource.Obsidian, 0),
				(Resource.Geode, 0))
			);
		
		// var paths = new List<State> { state };

		var paths = getPaths(state, null)
			.OrderBy(x => x.Resources.GetValue(Resource.Geode))
			.ThenBy(x => x.Resources.GetValue(Resource.Obsidian))
			.ThenBy(x => x.Resources.GetValue(Resource.Clay))
			.ThenBy(x => x.Resources.GetValue(Resource.Ore))
			.ToList();
		
		foreach (var path in paths.Where(x => x.Resources.GetValue(Resource.Geode) > 5))
			Console.WriteLine(path);

		return blueprint.Id * paths
			.Select(x => x.Resources.GetValue(Resource.Geode))
			.Max();

		IEnumerable<State> getPaths(State state, Resource? resourceToCreate)
		{
			if (resourceToCreate is not null)
			{
				int maxRobots = resourceToCreate.Value switch
				{
					Resource.Ore => 2,
					Resource.Clay => 3,
					Resource.Obsidian => 5,
					Resource.Geode => int.MaxValue,
					_ => throw new ArgumentOutOfRangeException(),
				};

				if (state.Robots.GetValue(resourceToCreate.Value) >= maxRobots)
					yield break;

				state = state.Clone();
				var robotCost = blueprint.RobotCosts.GetValue(resourceToCreate.Value);
				foreach (var (resource, cost) in robotCost.GetValues().Where(x => x.value > 0))
				{
					if (state.Resources.GetValue(resource) < cost)
						yield break;
					state.AdjustResources(resource, -cost);
				}
			}

			state.AdjustMinutes(1);

			foreach (var (resource, robotCount) in state.Robots.GetValues().Where(x => x.value > 0))
				state.AdjustResources(resource, robotCount);

			if (resourceToCreate is not null)
				state.AdjustRobots(resourceToCreate.Value, 1);

			if (state.Minutes < totalMinutes)
			{
				foreach (var x in getPaths(state, Resource.Geode))
					yield return x;

				foreach (var x in getPaths(state, Resource.Obsidian))
					yield return x;
				
				foreach (var x in getPaths(state, Resource.Clay))
					yield return x;
				
				foreach (var x in getPaths(state, Resource.Ore))
					yield return x;

				foreach (var x in getPaths(state, null))
					yield return x;
			}
			else
			{
				// if (state.Resources.GetValue(Resource.Geode) > 7)
				// 	Console.WriteLine(state);
				yield return state;
			}

			// int minutesDiff = totalMinutes - state.Minutes;
			// state.AdjustMinutes(minutesDiff);

			// foreach (var (resource, robotCount) in state.Robots.GetValues().Where(x => x.value > 0))
			// 	state.AdjustResources(resource, robotCount * minutesDiff);

			// Console.WriteLine(state);
			// yield return state;

			// Console.WriteLine(state);
			// if (minutesDiff > 0)
			// {
			// 	Console.WriteLine("recurse");
			// 	foreach (var x in getPaths(state, Resource.Geode))
			// 		yield return x;

			// 	foreach (var x in getPaths(state, Resource.Obsidian))
			// 		yield return x;
				
			// 	foreach (var x in getPaths(state, Resource.Clay))
			// 		yield return x;
				
			// 	foreach (var x in getPaths(state, Resource.Ore))
			// 		yield return x;
			// 	// something(state, Resource.Obsidian);
			// 	// something(state, Resource.Clay);
			// 	// something(state, Resource.Ore);
			// 	// something(state, null);
			// }
			// else
			// {
			// 	// Console.WriteLine(state);
			// }
		}
	}

	private enum Resource : int { Ore = 0, Clay = 1, Obsidian = 2, Geode = 3 };
	private record Blueprint(int Id, ResourceValues<ResourceValues<int>> RobotCosts);
	private class ResourceValues<T>
	{
		public ResourceValues(params (Resource, T)[] values)
		{
			m_values = values.ToDictionary(x => x.Item1, x => x.Item2);
		}

		public ResourceValues<T> Clone() => new ResourceValues<T>(GetValues().ToArray());

		public IEnumerable<(Resource resource, T value)> GetValues()
			=> m_values.Select(x => (x.Key, x.Value));

		public T GetValue(Resource resource) => m_values[resource];

		public void SetValue(Resource resource, Func<T, T> getNewValue)
		{
			var value = m_values[resource];
			m_values[resource] = getNewValue(value);

		}

		public override string ToString() => string.Join(", ", GetValues());

		private readonly Dictionary<Resource, T> m_values;
	}

	private class State
	{
		public State(int minutes, ResourceValues<int> robots, ResourceValues<int> resources)
		{
			Minutes = minutes;
			Robots = robots;
			Resources = resources;
		}

		public int Minutes { get; private set; }
		public ResourceValues<int> Robots { get; private set; }
		public ResourceValues<int> Resources { get; private set; }

		public void AdjustMinutes(int by) => Minutes += by;
		public void AdjustRobots(Resource resource, int by) => Robots.SetValue(resource, x => x + by);
		public void AdjustResources(Resource resource, int by) => Resources.SetValue(resource, x => x + by);
		public State Clone() => new State(Minutes, Robots.Clone(), Resources.Clone());
		public override string ToString() => $"m:{Minutes}, rob:{Robots}, res:{Resources}";
	}
}