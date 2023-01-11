using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

namespace AdventOfCode2022;

public class Day16 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var model = new Model(input);
		yield return model.CalculateMaxPressureReleased(30).ToString();
	}

	private record Valve(string Id, uint NodeId, int FlowRate)
	{
		public override string ToString() => $"{Id}:{FlowRate:D2}";
	}

	private class Model
	{
		public Model(IReadOnlyList<string> input)
		{
			m_graph = new Graph();
			m_valves = input
				.Select(x => x.Split(' '))
				.Select(x => new Valve(x[1], m_graph.AddNode(), int.Parse(x[4].Split('=')[1].Trim(';'))))
				.ToDictionary(x => x.NodeId);
			
			foreach (var tokens in input.Select(x => x.Split(' ')))
			{
				var fromValve = GetValveById(tokens[1]);
				foreach (var toValve in tokens.Skip(9).Select(x => GetValveById(x.Trim(','))))
					m_graph.Connect(fromValve.NodeId, toValve.NodeId, 1);
			}
		}

		public int CalculateMaxPressureReleased(int minutes)
		{
			int maxPressureReleased = 0;
			var goodValves = m_valves.Values.Where(x => x.FlowRate > 0).ToArray();
			calculateMaxPressureReleased(GetValveById("AA"), Array.Empty<Valve>(), 0, minutes);
			return maxPressureReleased;

			void calculateMaxPressureReleased(Valve currentValve, IReadOnlyList<Valve> currentPath, int pressureReleased, int timeRemaining )
			{
				// We need at least three minutes to open a valve and get some value from it.
				if (timeRemaining < 3)
					return;

				var availableValves = goodValves
					.Except(currentPath)
					.OrderByDescending(x => x.FlowRate)
					.ToList();

				if (availableValves.Count == 0)
					return;

				int maxPotentialPresureReleased = pressureReleased + availableValves
					.OrderByDescending(x => x.FlowRate)
					.Select((x, i) => x.FlowRate * (timeRemaining - (2 * (i + 1))))
					.TakeWhile(x => x > 0)
					.Sum();
				
				// Bail if this path can't possibly win, even with the best possible node arrangement.
				if (maxPotentialPresureReleased <= maxPressureReleased)
					return;

				foreach (var nextValve in availableValves)
				{
					var path = GetPath(currentValve, nextValve);

					// Bail if we can't get to the valve before time runs out.
					int nextTimeRemaining = timeRemaining - path.Count;
					if (nextTimeRemaining <= 0)
						continue;

					// Bail if there is a valve with a greater flow rate than the destination valve.
					// We know opening up that valve instead would be a better path.
					if (nextValve != path.Where(availableValves.Contains).MaxBy(x => x.FlowRate))
						continue;

					var nextCurrentPath = currentPath.Append(nextValve).ToList();
					int nextPressureReleased = pressureReleased + (nextValve.FlowRate * nextTimeRemaining);

					if (nextPressureReleased > maxPressureReleased)
						maxPressureReleased = nextPressureReleased;

					calculateMaxPressureReleased(nextValve, nextCurrentPath, nextPressureReleased, nextTimeRemaining);
				}
			}
		}

		private Valve GetValveById(string id) => m_valves.Values.Single(x => x.Id == id);

		private IReadOnlyList<Valve> GetPath(Valve from, Valve to)
		{
			if (!m_pathCache.TryGetValue((from.NodeId, to.NodeId), out var path))
			{
				var result = m_graph.Dijkstra(from.NodeId, to.NodeId);
				m_pathCache[(from.NodeId, to.NodeId)] = path = result.GetPath().Select(nodeId => m_valves[nodeId]).ToList();
			}
			return path;
		}

		private readonly Graph m_graph;
		private readonly IReadOnlyDictionary<uint, Valve> m_valves;
		private readonly Dictionary<(uint, uint), IReadOnlyList<Valve>> m_pathCache = new Dictionary<(uint, uint), IReadOnlyList<Valve>>();
	}
}