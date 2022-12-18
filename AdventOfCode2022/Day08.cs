namespace AdventOfCode2022;

public class Day08 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var treeGrid = new TreeGrid(input.SelectMany((line, y) => line.Select((ch, x) => (new Vector(x, y), int.Parse(ch.ToString())))));

		yield return treeGrid.CalculateNumberOfVisibleTrees().ToString();
		yield return treeGrid.CalculateMaxScenicScore().ToString();
	}

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
				!GetTreesInDirection(Vector.Up).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector.Right).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector.Down).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector.Left).Any(x => x.Height >= Height);
			
			public int ScenicScore =>
				CalculateViewingDistance(Vector.Up) *
				CalculateViewingDistance(Vector.Right) *
				CalculateViewingDistance(Vector.Down) *
				CalculateViewingDistance(Vector.Left);

			private IEnumerable<Tree> GetTreesInDirection(Vector direction)
			{
				Vector nextLocation = Location;
				while (true)
				{
					nextLocation = nextLocation.Add(direction);
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
