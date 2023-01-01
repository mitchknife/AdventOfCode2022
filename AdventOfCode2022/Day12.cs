using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

namespace AdventOfCode2022;

public class Day12 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var map = new Map(input);
		var start = map.GetCells('S').Single();
		var finish = map.GetCells('E').Single();
		yield return map.GetShortestNumberOfSteps(start.Location, finish.Location).ToString();
		yield return map.GetCells('S', 'a')
			.Select(x => map.GetShortestNumberOfSteps(x.Location, finish.Location))
			.Where(x => x > 0).Min().ToString();
	}

	private class Map
	{
		public Map(IReadOnlyList<string> input)
		{
			m_graph = new Graph();
			m_nodeIdsByLocation = new Dictionary<Vector2D, uint>();
			m_cellsByNodeId = new Dictionary<uint, MapCell>();

			foreach (var cell in input.SelectMany((line, y) => line.Select((ch, x) => new MapCell(new Vector2D(x, y), ch))))
			{
				var nodeId = m_graph.AddNode();
				m_nodeIdsByLocation.Add(cell.Location, nodeId);
				m_cellsByNodeId.Add(nodeId, cell);
			}

			var directions = new Vector2D[] { Vector2D.Up, Vector2D.Down, Vector2D.Right, Vector2D.Left };
			foreach (var fromNodeId in m_nodeIdsByLocation.Values)
			{
				var fromCell = m_cellsByNodeId[fromNodeId];
				foreach (var direction in directions)
				{
					var toLocation = fromCell.Location + direction;
					var toNodeId = m_nodeIdsByLocation.GetValueOrDefault(toLocation);
					if (toNodeId > 0)
					{
						var toCell = m_cellsByNodeId[toNodeId];
						int edgeCost = fromCell.Height - toCell.Height + 2;
						if (edgeCost > 0)
							m_graph.Connect(fromNodeId, toNodeId, edgeCost);
					}
				}
			}
		}

		public IReadOnlyList<MapCell> GetCells(params char[] chars)
			=> m_cellsByNodeId.Values.Where(cell => chars.Contains(cell.Char)).ToList();

		public int GetShortestNumberOfSteps(Vector2D from, Vector2D to)
		{
			var startNodeId = m_nodeIdsByLocation[from];
			var finishNodeId = m_nodeIdsByLocation[to];
			var result = m_graph.Dijkstra(startNodeId, finishNodeId);
			return result.GetPath().Skip(1).Select(x => m_cellsByNodeId[x]).Count();
		}

		private readonly Graph m_graph;
		private readonly Dictionary<Vector2D, uint> m_nodeIdsByLocation;
		private readonly Dictionary<uint, MapCell> m_cellsByNodeId;
	}
	private record MapCell(Vector2D Location, char Char)
	{
		public int Height => Char == 'S' ? 'a' : Char == 'E' ? 'z' : Char;
	}
}
