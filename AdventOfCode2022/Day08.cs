namespace AdventOfCode2022;

public class Day08 : IDay
{
	public IReadOnlyList<string> Execute(IReadOnlyList<string> input)
	{
		var treeGrid = new TreeGrid(input.SelectMany((line, y) => line.Select((ch, x) => (new Vector(x, y), int.Parse(ch.ToString())))));

		return new[]
		{
			treeGrid.CalculateNumberOfVisibleTrees().ToString(),
			treeGrid.CalculateMaxScenicScore().ToString(),
		};
	}

	private sealed record Vector(int X, int Y);

	private sealed class TreeGrid
	{
		public TreeGrid(IEnumerable<(Vector, int)> trees)
		{
			foreach (var (location, height) in trees)
			{
				m_maxX = Math.Max(m_maxX, location.X);
				m_maxY = Math.Max(m_maxY, location.Y);
				m_trees.Add(location, new Tree(location, height, this));
			}
		}

		public int CalculateNumberOfVisibleTrees() => m_trees.Values.Count(x => x.IsVisible);
		public int CalculateMaxScenicScore() => m_trees.Values.Select(x => x.ScenicScore).Max();

		private record Tree(Vector Location, int Height, TreeGrid TreeGrid)
		{
			public bool IsVisible =>
				!GetTreesInDirection(new(0, -1)).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(new(1, 0)).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(new(0, 1)).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(new(-1, 0)).Any(x => x.Height >= Height);
			
			public int ScenicScore =>
				CalculateViewingDistance(new(0, -1)) *
				CalculateViewingDistance(new(1, 0)) *
				CalculateViewingDistance(new(0, 1)) *
				CalculateViewingDistance(new(-1, 0));

			private IEnumerable<Tree> GetTreesInDirection(Vector direction)
			{
				Vector nextLocation = Location;
				while (true)
				{
					nextLocation = new Vector(nextLocation.X + direction.X, nextLocation.Y + direction.Y);
					var tree = TreeGrid.m_trees.GetValueOrDefault(nextLocation);
					if (tree is null)
						break;

					yield return tree;
				}
			}

			private int CalculateViewingDistance(Vector direction)
			{
				int viewDistance = 0;
				foreach (var tree in GetTreesInDirection(direction))
				{
					viewDistance++;
					if (tree.Height >= Height)
						break;
				}
				return viewDistance;
			}
		}

		private int m_maxX;
		private int m_maxY;
		private readonly Dictionary<Vector, Tree> m_trees = new Dictionary<Vector, Tree>();
	}
}
