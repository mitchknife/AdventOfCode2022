namespace AdventOfCode2022;

public class Day06 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		string buffer = input.Single();
		yield return FindMarker(buffer, 4).ToString();
		yield return FindMarker(buffer, 14).ToString();
	}

	private static int FindMarker(string buffer, int count)
	{
		var queue = new Queue<char>();
		foreach (var (ch, processed) in buffer.Select((x, i) => (x, i + 1)))
		{
			if (queue.Count == count)
				queue.Dequeue();
			
			queue.Enqueue(ch);
			if (queue.Distinct().Count() == count)
				return processed;
		}

		return 0;
	}
}
