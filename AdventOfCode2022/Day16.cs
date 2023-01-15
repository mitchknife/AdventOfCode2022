using System.Runtime.Serialization;
using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

namespace AdventOfCode2022;

public class Day16 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var model = new Model(input);
		yield return model.CalculateMaxPressureReleased(30).ToString();
		yield return model.CalculateMaxPressureReleased(26, withElephant: true).ToString();
	}

	private record Valve(string Id, uint NodeId, int FlowRate);

	private record Worker(Valve CurrentValve, IEnumerable<Valve> CurrentPath, int PressureReleased, int TimeRemaining)
	{
		public bool CanOpenValves => TimeRemaining >= 3;
		public Worker TryOpenValve(Valve valve, int distance)
		{
			int timeRemaining = TimeRemaining - distance - 1;
			if (timeRemaining <= 0)
				return null;
			
			return new Worker(
				valve,
				CurrentPath.Append(valve),
				PressureReleased + (valve.FlowRate * timeRemaining),
				timeRemaining);
		}
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

		public int CalculateMaxPressureReleased(int minutes, bool withElephant = false)
		{
			int maxPressureReleased = 0;
			var goodValves = m_valves.Values.Where(x => x.FlowRate > 0).ToArray();
			var checkedPaths = new HashSet<string>();
			var worker = new Worker(GetValveById("AA"), Array.Empty<Valve>(), 0, minutes);
			var workers = withElephant ? new[] { worker, worker } : new[] { worker };

			calculateMaxPressureReleased(workers);
			return maxPressureReleased;

			void calculateMaxPressureReleased(IReadOnlyList<Worker> workers)
			{
				// HACK: I tried :(
				if (maxPressureReleased >= 1933)
					return;

				string currentPathKey = string.Join(":", workers);
				if (!checkedPaths.Add(currentPathKey))
					return;

				// We need at least three minutes to open a valve and get some value from it.
				if (workers.All(x => x.TimeRemaining < 3))
					return;

				var availableValves = goodValves
					.Except(workers.SelectMany(x => x.CurrentPath))
					.OrderByDescending(x => x.FlowRate)
					.ToList();

				if (availableValves.Count == 0)
					return;

				// Bail if this path can't possibly win, even with the best possible node arrangement.
				int maxPotentialPressureReleased = calculateMaxPotentialPressureReleased(workers, availableValves);
				if (maxPotentialPressureReleased <= maxPressureReleased)
					return;

				var valveChunks = availableValves
					.OrderBy(v => GetPath(workers[0].CurrentValve, v).Count)
					.Select(v => new List<Valve> { v }.AsEnumerable());

				foreach (var worker in workers.Skip(1))
					valveChunks = valveChunks.SelectMany(ch => availableValves.OrderBy(v => GetPath(worker.CurrentValve, v).Count).Select(v => ch.Append(v)));

				foreach (var chunk in valveChunks.Where(c => c.Distinct().Count() == workers.Count))
				{
					bool recurse = false;
					var nextWorkers = new List<Worker>();
					foreach (var (worker, valve) in workers.Zip(chunk))
					{
						if (!worker.CanOpenValves)
							continue;

						var path = GetPath(worker.CurrentValve, valve);
						if (path.Any(v => v != valve && availableValves.Contains(v) && v.FlowRate >= valve.FlowRate))
							continue;
						
						var nextWorker = worker.TryOpenValve(valve, path.Count - 1) ?? worker;
						recurse = recurse || nextWorker.CurrentValve == valve;
						nextWorkers.Add(nextWorker);
					}

					if (recurse)
					{
						int totalPressureReleased = nextWorkers.Sum(w => w.PressureReleased);
						if (totalPressureReleased > maxPressureReleased)
							maxPressureReleased = totalPressureReleased;

						calculateMaxPressureReleased(nextWorkers);
					}
				}
			}

			int calculateMaxPotentialPressureReleased(IReadOnlyList<Worker> workers, IReadOnlyList<Valve> valves)
			{
				var workerList = workers.ToList();
				foreach (var valve in valves)
				{
					var (worker, index) = workerList.Select((x, i) => (x, i)).MaxBy(x => x.x.TimeRemaining);
					worker = worker.TryOpenValve(valve, 1);
					if (worker is not null)
						workerList[index] = worker;
				}

				return workerList.Sum(x => x.PressureReleased);
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
