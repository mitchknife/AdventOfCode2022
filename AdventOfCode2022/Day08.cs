namespace AdventOfCode2022;

public class Day08 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var treeGrid = new TreeGrid(input.SelectMany((line, y) => line.Select((ch, x) => (new Vector2D(x, y), int.Parse(ch.ToString())))));

		yield return treeGrid.CalculateNumberOfVisibleTrees().ToString();
		yield return treeGrid.CalculateMaxScenicScore().ToString();
	}

	private sealed class TreeGrid
	{
		public TreeGrid(IEnumerable<(Vector2D, int)> trees)
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

		private record Tree(Vector2D Location, int Height, TreeGrid TreeGrid)
		{
			public bool IsVisible =>
				!GetTreesInDirection(Vector2D.Up).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector2D.Right).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector2D.Down).Any(x => x.Height >= Height) ||
				!GetTreesInDirection(Vector2D.Left).Any(x => x.Height >= Height);
			
			public int ScenicScore =>
				CalculateViewingDistance(Vector2D.Up) *
				CalculateViewingDistance(Vector2D.Right) *
				CalculateViewingDistance(Vector2D.Down) *
				CalculateViewingDistance(Vector2D.Left);

			private IEnumerable<Tree> GetTreesInDirection(Vector2D direction)
			{
				Vector2D nextLocation = Location;
				while (true)
				{
					nextLocation += direction;
					var tree = TreeGrid.m_trees.GetValueOrDefault(nextLocation);
					if (tree is null)
						break;

					yield return tree;
				}
			}

			private int CalculateViewingDistance(Vector2D direction)
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
		private readonly Dictionary<Vector2D, Tree> m_trees = new Dictionary<Vector2D, Tree>();
	}
}
