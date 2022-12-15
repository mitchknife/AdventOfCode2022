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
				"U" => new Vector(0, 1),
				"R" => new Vector(1, 0),
				"D" => new Vector(0, -1),
				"L" => new Vector(-1, 0),
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

	private record Vector(int X, int Y);

	private class Rope
	{
		public Rope(int numberOfSegments)
		{
			m_segments = Enumerable.Range(0, numberOfSegments).Select(_ => new Vector(0, 0)).ToList();
			m_uniqueTailLocations = new HashSet<Vector>(new[] { new Vector(0,0) });
		}

		public void MoveHead(Vector direction)
		{
			m_segments[0] = new Vector(m_segments[0].X + direction.X, m_segments[0].Y + direction.Y);
			for (int i = 1; i < m_segments.Count; i++)
			{
				var segStart = m_segments[i - 1];
				var segEnd = m_segments[i];
				int xDiff = segStart.X - segEnd.X;
				int yDiff = segStart.Y - segEnd.Y;
				if (Math.Abs(xDiff) == 2 || Math.Abs(yDiff) == 2)
				{
					int moveX = xDiff == 0 ? 0 : xDiff / Math.Abs(xDiff);
					int moveY = yDiff == 0 ? 0 : yDiff / Math.Abs(yDiff);
					m_segments[i] = new Vector(segEnd.X + moveX, segEnd.Y + moveY);
				}
			}

			m_uniqueTailLocations.Add(m_segments.Last());
		}

		public int GetNumberOfUniqueTailLocations() => m_uniqueTailLocations.Count();

		private readonly List<Vector> m_segments = new List<Vector>();
		private readonly HashSet<Vector> m_uniqueTailLocations = new HashSet<Vector>();
	}
}
