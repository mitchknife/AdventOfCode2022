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
			.Select(ints => new Blueprint(ints[0], ResourceValues<ResourceValues<int>>.Create(
				ore: ResourceValues<int>.Create(ore: ints[1]),
				clay: ResourceValues<int>.Create(ore: ints[2]),
				obsidian: ResourceValues<int>.Create(ore: ints[3], clay: ints[4]),
				geode: ResourceValues<int>.Create(ore: ints[5], obsidian: ints[6]))))
			.ToList();

		yield return blueprints
			.Select(blueprint => CalculateMaxGeodeScore(blueprint, 24) * blueprint.Id)
			.Sum()
			.ToString();
		
		yield return blueprints
			.Take(3)
			.Select(blueprint => CalculateMaxGeodeScore(blueprint, 32))
			.Aggregate(1, (x, y) => x * y)
			.ToString();

	}

	private int CalculateMaxGeodeScore(Blueprint blueprint, int totalMinutes)
	{
		var state = new State(
			blueprint: blueprint,
			robots: ResourceValues<int>.Create(ore: 1),
			resources: ResourceValues<int>.Create(),
			skipResources: ResourceValues<bool>.Create(),
			minutes: totalMinutes);

		int maxGeodeScore = 0;
		calculateMaxGeodeScore(state, null);
		return maxGeodeScore;

		void calculateMaxGeodeScore(State state, string robotToBuild)
		{
			if (robotToBuild is not null)
			{
				if (!state.CanBuildRobot(robotToBuild))
					return;

				if (!state.ShouldBuildRobot(robotToBuild))
					return;

				state = state.Clone();
				var robotCost = state.Blueprint.RobotCosts.GetValue(robotToBuild);
				foreach (var (resource, cost) in robotCost.GetValues().Where(x => x.Value > 0))
					state.Resources.SetValue(resource, x => x - cost);

				foreach (string resource in Resources.All.Where(x => x != robotToBuild))
					state.SkipResources.SetValue(resource, value => value ? !value : state.CanBuildRobot(resource));
			}
			else
			{
				foreach (string resource in Resources.All.Where(state.CanBuildRobot))
					state.SkipResources.SetValue(resource, _ => true);
			}

			state.Minutes--;

			foreach (var (resource, robotCount) in state.Robots.GetValues().Where(x => x.Value > 0))
				state.Resources.SetValue(resource, x => x + robotCount);

			if (robotToBuild is not null)
				state.Robots.SetValue(robotToBuild, x => x + 1);

			if (maxGeodeScore > 0)
			{
				int bestPossibleGeodeScore = state.Resources.GetValue(Resources.Geode) +
					Enumerable.Range(state.Robots.GetValue(Resources.Geode), state.Minutes).Sum();
				
				if (bestPossibleGeodeScore <= maxGeodeScore)
					return;
			}

			if (state.Minutes > 0)
			{
				calculateMaxGeodeScore(state, Resources.Geode);
				calculateMaxGeodeScore(state, Resources.Obsidian);
				calculateMaxGeodeScore(state, Resources.Clay);
				calculateMaxGeodeScore(state, Resources.Ore);
				calculateMaxGeodeScore(state, null);
			}
			else
			{
				maxGeodeScore = Math.Max(maxGeodeScore, state.Resources.GetValue(Resources.Geode));
			}
		}
	}

	private static class Resources
	{
		public const string Ore = nameof(Ore);
		public const string Clay = nameof(Clay);
		public const string Obsidian = nameof(Obsidian);
		public const string Geode = nameof(Geode);
		public static IReadOnlyList<string> All = new[] { Ore, Clay, Obsidian, Geode };
	}

	private record Blueprint(int Id, ResourceValues<ResourceValues<int>> RobotCosts)
	{
		public ResourceValues<int> MaxResourcesRates = ResourceValues<int>.Create(
			ore: RobotCosts.GetValues().Select(x => x.Value.GetValue(Resources.Ore)).Max(),
			clay: RobotCosts.GetValues().Select(x => x.Value.GetValue(Resources.Clay)).Max(),
			obsidian: RobotCosts.GetValues().Select(x => x.Value.GetValue(Resources.Obsidian)).Max(),
			geode: int.MaxValue);
	}

	private class ResourceValues<T>
	{
		public static ResourceValues<T> Create(T ore = default, T clay = default, T obsidian = default, T geode = default)
		{
			return new ResourceValues<T>(new Dictionary<string, T>
			{
				[Resources.Ore] = ore,
				[Resources.Clay] = clay,
				[Resources.Obsidian] = obsidian,
				[Resources.Geode] = geode,
			});
		}

		private ResourceValues(Dictionary<string, T> values)
		{
			m_values = values;
		}

		public ResourceValues<T> Clone() => new ResourceValues<T>(m_values.ToDictionary(x => x.Key, x => x.Value));

		public IEnumerable<(string Resource, T Value)> GetValues() => m_values.Select(x => (x.Key, x.Value));

		public T GetValue(string resource) => m_values[resource];

		public void SetValue(string resource, Func<T, T> getNewValue)
		{
			var value = m_values[resource];
			m_values[resource] = getNewValue(value);
		}

		private readonly Dictionary<string, T> m_values;
	}

	private class State
	{
		public State(Blueprint blueprint, ResourceValues<int> robots, ResourceValues<int> resources, ResourceValues<bool> skipResources, int minutes)
		{
			Blueprint = blueprint;
			Robots = robots;
			Resources = resources;
			SkipResources = skipResources;
			Minutes = minutes;
		}

		public Blueprint Blueprint { get; init; }
		public ResourceValues<int> Robots { get; private set; }
		public ResourceValues<int> Resources { get; private set; }
		public ResourceValues<bool> SkipResources { get; init; }
		public int Minutes { get; set; }

		public bool CanBuildRobot(string resource) => Blueprint.RobotCosts
			.GetValue(resource)
			.GetValues()
			.Where(x => x.Value > 0)
			.All(x => Resources.GetValue(x.Resource) >= x.Value);

		public bool ShouldBuildRobot(string resource)
		{
			if (SkipResources.GetValue(resource))
				return false;

			int maxResourceRate = Blueprint.MaxResourcesRates.GetValue(resource);
			if (Robots.GetValue(resource) >= maxResourceRate)
				return false;

			var geodeRobotCost = Blueprint.RobotCosts
				.GetValue(Day19.Resources.Geode)
				.GetValues().Where(x => x.Value > 0)
				.Select(x => x.Resource)
				.ToList();
			
			int minutesLeftToBuildRobots = Minutes - (
				resource == Day19.Resources.Geode ? 1 :
				geodeRobotCost.Contains(resource) ? 2 :
				3);

			if (minutesLeftToBuildRobots <= 0)
				return false;

			return true;
		}

		public State Clone() => new State(Blueprint, Robots.Clone(), Resources.Clone(), SkipResources.Clone(), Minutes);
	}
}