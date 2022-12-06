namespace AdventOfCode2022;

public class Day06 : IDay
{
	public IReadOnlyList<string> Execute(IReadOnlyList<string> input)
	{
		var output = new List<string>();
		var counts = new[] { 4, 14 };

		foreach (string line in input)
		{
			var queuesWithCounts = counts
				.Select(count => (queue: new Queue<char>(), count))
				.ToList();

			foreach (var (ch, processed) in line.Select((x, i) => (x, i + 1)))
			{
				foreach (var (queue, count) in queuesWithCounts.ToList())
				{
					if (queue.Count == count)
						queue.Dequeue();

					queue.Enqueue(ch);
					if (queue.Distinct().Count() == count)
					{
						queuesWithCounts.Remove((queue, count));
						output.Add(processed.ToString());
					}
				}
			}
		}

		return output;
	}
}
