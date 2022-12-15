namespace AdventOfCode2022;

public class Day03 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var rucksacks = input.Select(line => new Rucksack(
			Comp1: line.Substring(0, line.Length / 2).Select(ch => new Item(ch)).ToList(),
			Comp2: line.Substring(line.Length / 2).Select(ch => new Item(ch)).ToList()
		)).ToList();

		var rucksackGroups = rucksacks
			.Select((rucksack, index) => (rucksack, index))
			.GroupBy(g => g.index / 3)
			.Select(g => new RucksackGroup(g.Select(x => x.rucksack).ToList()))
			.ToList();

		yield return rucksacks.Sum(r => r.GetCommonItem().Priority).ToString();
		yield return rucksackGroups.Sum(r => r.GetCommonItem().Priority).ToString();
	}

	record Item(char Value)
	{
		public int Priority => char.IsLower(Value) ? (int) Value - 96 : (int) Value - 38;
	}

	record Rucksack(IReadOnlyList<Item> Comp1, IReadOnlyList<Item> Comp2)
	{
		public Item GetCommonItem() => Comp1.Intersect(Comp2).Single();
		public IReadOnlyList<Item> GetAllItems() => Comp1.Concat(Comp2).Distinct().ToList();
	}

	record RucksackGroup(IReadOnlyList<Rucksack> Rucksacks)
	{
		public Item GetCommonItem()
		{
			var commonItems = new HashSet<Item>(Rucksacks.First().GetAllItems());
			foreach (var rucksack in Rucksacks.Skip(1))
				commonItems.IntersectWith(rucksack.GetAllItems());
			return commonItems.Single();
		}
	}

}