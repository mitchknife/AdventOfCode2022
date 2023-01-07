namespace AdventOfCode2022;

public class Day20 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var values = input.Select(int.Parse).ToList();
		yield return CalculateCoordinates(values, 1, 1).Sum().ToString();
		yield return CalculateCoordinates(values, 10, 811589153).Sum().ToString();
	}

	public IReadOnlyList<long> CalculateCoordinates(IReadOnlyList<int> values, int mixCount, long decryptionKey)
	{
		var originalItems = values.Select((x, i) => new Item(i, x * decryptionKey)).ToArray();
		var items = originalItems.ToList();
		var indexes = items.ToDictionary(x => x.Id, x => x.Id);

		int minIndex = 0;
		int maxIndex = items.Count - 1;
		foreach (var round in Enumerable.Range(1, mixCount))
		{
			foreach (var item in originalItems)
			{
				if (item.Value is 0)
					continue;

				int index = indexes[item.Id];
				int newIndex = index + (int) (item.Value % maxIndex);
				if (newIndex <= minIndex)
					newIndex = items.Count + (newIndex - 1);
				else if (newIndex >= maxIndex)
					newIndex = (newIndex + 1) % items.Count;

				int step = newIndex > index ? 1 : -1;
				for (int i = index; i != newIndex; i += step)
				{
					var nextItem = items[i + step];
					indexes[nextItem.Id] = i;
					items[i] = nextItem;
				}

				items[newIndex] = item;
				indexes[item.Id] = newIndex;
			}
		}

		int indexOfZero = indexes[items.Single(x => x.Value == 0).Id];
		return new[] { 1000, 2000, 3000 }
			.Select(x => items[(indexOfZero + x) % items.Count].Value)
			.ToList();
	}

	private record Item(int Id, long Value);
}