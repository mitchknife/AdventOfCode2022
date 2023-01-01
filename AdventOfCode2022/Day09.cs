namespace AdventOfCode2022;

public class Day09 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var rope1 = new Rope(2);
		var rope2 = new Rope(10);
		foreach (var tokens in input.Select(x => x.Split(' ')))
		{
			var direction = tokens[0] switch
			{
				"U" => (0, 1),
				"R" => (1, 0),
				"D" => (0, -1),
				"L" => (-1, 0),
				_ => throw new ArgumentOutOfRangeException(),
			};
			
			foreach (var _ in Enumerable.Range(0, int.Parse(tokens[1])))
			{
				rope1.MoveHead(direction);
				rope2.MoveHead(direction);
			}
		}

		yield return rope1.GetNumberOfUniqueTailLocations().ToString();
		yield return rope2.GetNumberOfUniqueTailLocations().ToString();
	}

	private class Rope
	{
		public Rope(int numberOfSegments)
		{
			m_segments = Enumerable.Range(0, numberOfSegments).Select(_ => Vector2D.Zero).ToList();
			m_uniqueTailLocations = new HashSet<Vector2D>(new[] { Vector2D.Zero });
		}

		public void MoveHead(Vector2D direction)
		{
			m_segments[0] = m_segments[0].Add(direction);
			for (int i = 1; i < m_segments.Count; i++)
			{
				var segStart = m_segments[i - 1];
				var segEnd = m_segments[i];
				var (xDiff, yDiff) = segStart - segEnd;
				if (Math.Abs(xDiff) == 2 || Math.Abs(yDiff) == 2)
				{
					int moveX = xDiff == 0 ? 0 : xDiff / Math.Abs(xDiff);
					int moveY = yDiff == 0 ? 0 : yDiff / Math.Abs(yDiff);
					m_segments[i] = segEnd + (moveX, moveY);
				}
			}

			m_uniqueTailLocations.Add(m_segments.Last());
		}

		public int GetNumberOfUniqueTailLocations() => m_uniqueTailLocations.Count();

		private readonly List<Vector2D> m_segments = new List<Vector2D>();
		private readonly HashSet<Vector2D> m_uniqueTailLocations = new HashSet<Vector2D>();
	}
}
