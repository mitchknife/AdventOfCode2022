using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

namespace AdventOfCode2022;

public class Day16 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var valves = input
			.Select(x => x.Split(' '))
			.Select(x => new Valve(
				Id: x[1],
				FlowRate: int.Parse(x[4].Split('=')[1].Trim(';')),
				AdjacentValveIds: x.Skip(9).Select(x => x.Trim(',')).ToList()))
			.ToList();

		var graph = new Graph();
		var valvesByNodeId = new Dictionary<uint, Valve>();
		var nodeIdsByValveId = new Dictionary<string, uint>();
		foreach (var valve in valves)
		{
			var nodeId = graph.AddNode();
			valvesByNodeId.Add(nodeId, valve);
			nodeIdsByValveId.Add(valve.Id, nodeId);
		}
		
		foreach (var pair in valvesByNodeId)
		{
			foreach (var adjacentNodeId in pair.Value.AdjacentValveIds.Select(id => nodeIdsByValveId[id]))
				graph.Connect(pair.Key, adjacentNodeId, 1);
		}

		yield return CalculateMaxPressureRelease(valves.First(), 30, 0).ToString();

		int CalculateMaxPressureRelease(Valve fromValve, int minutesLeft, int currentPressure)
		{
			var bestPaths = valves
				.Where(x => x.Id != fromValve.Id && !x.IsOpen)
				.Select(v => (valve: v, result: graph.Dijkstra(nodeIdsByValveId[fromValve.Id], nodeIdsByValveId[v.Id])))
				.Select(x => (valve: x.valve, totalRelease: x.valve.FlowRate * (minutesLeft - x.result.Distance - 1), distance: x.result.Distance))
				.Where(x => x.totalRelease > 0)
				.OrderByDescending(x => Math.Ceiling((decimal) x.valve.FlowRate / (decimal) x.distance))
				.ThenBy(x => x.distance)
				.ToList();
			
			if (bestPaths.Count == 0)
				return currentPressure;
			
			var (toValve, totalRelease, distance) = bestPaths.First();
			minutesLeft = minutesLeft - distance - 1;
			toValve.IsOpen = true;
			currentPressure += totalRelease;

			return CalculateMaxPressureRelease(toValve, minutesLeft, currentPressure);
		}

		yield break;
	} 

	private record Valve(string Id, int FlowRate, IReadOnlyList<string> AdjacentValveIds)
	{
		public bool IsOpen { get; set; }	 
		public override string ToString() => $"{Id} | {FlowRate:D2} | {string.Join(", ", AdjacentValveIds)}";
	}
}