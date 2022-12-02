namespace AdventOfCode2022;

public class Day1 : Day
{
	public override IReadOnlyList<string> ExecuteCore(IReadOnlyList<string> input)
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

		var orderedElves = elves.OrderByDescending(x => x.TotalCalories).ToList();

		return new[]
		{
			orderedElves.Take(1).Sum(x => x.TotalCalories).ToString(),
			orderedElves.Take(3).Sum(x => x.TotalCalories).ToString(),
		};
	}

	record Elf(int TotalCalories);
}