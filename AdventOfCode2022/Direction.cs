namespace AdventOfCode2022;

public enum Direction
{
	Up,
	Right,
	Down,
	Left,
};

public static class DirectionExtensions
{
	public static Direction Rotate(this Direction direction, int numberOfTurns)
	{
		return DirectionsForever(numberOfTurns > 0)
			.SkipWhile(d => d != direction)
			.Skip(Math.Abs(numberOfTurns % 4))
			.First();
	}
	
	private static IEnumerable<Direction> DirectionsForever(bool clockwise = true)
	{
		var directions = clockwise ? m_clockwise.AsEnumerable() : m_clockwise.Reverse();
		while (true)
		{
			foreach (var direction in directions)
				yield return direction;
		}
	}
	
	private static readonly IReadOnlyList<Direction> m_clockwise = Enum.GetValues<Direction>();
}
