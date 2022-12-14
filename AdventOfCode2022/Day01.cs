namespace AdventOfCode2022;

public class Day01 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var elves = new List<Elf>();
		var currentItems = new List<int>();
		foreach (string line in input)
		{
			if (int.TryParse(line, out int item))
			{
				currentItems.Add(item);
			}
			else
			{
				elves.Add(new Elf(currentItems.Sum()));
				currentItems.Clear();
			}
		}

		if (currentItems.Any())
			elves.Add(new Elf(currentItems.Sum()));

		var orderedElves = elves.OrderByDescending(x => x.TotalCalories).ToList();

		yield return orderedElves.Take(1).Sum(x => x.TotalCalories).ToString();
		yield return orderedElves.Take(3).Sum(x => x.TotalCalories).ToString();
	}

	record Elf(int TotalCalories);
}