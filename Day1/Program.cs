namespace Day1;
class Program
{
	static void Main(string[] args)
	{
		var elves = new List<Elf>();
		var currentItems = new List<int>();
		foreach (string line in File.ReadAllLines("input.txt"))
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

		if (currentItems.Count > 0)
			elves.Add(new Elf(currentItems.Sum()));

		var orderedElves = elves.OrderByDescending(x => x.TotalCalories).ToList();
		Console.WriteLine($"Part 1: {orderedElves.Take(1).Sum(x => x.TotalCalories)}");
		Console.WriteLine($"Part 2: {orderedElves.Take(3).Sum(x => x.TotalCalories)}");
	}

	record Elf(int TotalCalories);
}
