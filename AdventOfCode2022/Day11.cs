namespace AdventOfCode2022;

public class Day11 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var part1Monkeys = ObserveMonkeys(input, 20, 3);

		yield return part1Monkeys.OrderByDescending(x => x.TotalItemsInspected).Take(2).Select(x => x.TotalItemsInspected).Aggregate(1, (x, y) => x * y).ToString();
	}

	static IReadOnlyList<Monkey> ObserveMonkeys(IEnumerable<string> input, int rounds, int worryModifier)
	{
		var monkeys = new List<Monkey>();
		foreach (var monkeyLines in input.Select((x, i) => (x, i)).GroupBy(x => x.i / 7, x => x.x, (key, items) => items.ToList()))
			Monkey.ParseAndAddMonkey(monkeyLines, monkeys, worryModifier);

		foreach (var round in Enumerable.Range(1, rounds))
		{
			foreach (var monkey in monkeys)
				monkey.InspectAllItems();
		}

		return monkeys;
	}

	private record Item(int WorryLevel);

	private class Monkey
	{
		public static void ParseAndAddMonkey(IReadOnlyList<string> monkeyLines, IList<Monkey> monkeys, int worryModifier)
		{
			int monkeyNumber = int.Parse(monkeyLines[0].Split(' ').Last().TrimEnd(':'));
			var initialItems = monkeyLines[1].Split(':')[1].Split(',').Select(x => new Item(int.Parse(x.Trim()))).ToList();
			
			var operationTokens = monkeyLines[2].Split('=')[1].Split(' ');
			Item adjustWorryLevel(Item item)
			{
				int rightSide = int.TryParse(operationTokens[3], out var i) ? i : item.WorryLevel;

				var next = operationTokens[2] == "*"
					? item.WorryLevel * rightSide
					: item.WorryLevel + rightSide;

				if (worryModifier > 1)
					next = next / worryModifier;

				return new Item(next);
			}

			Monkey pickNextMonkey(Item item)
			{
				int divisor = int.Parse(monkeyLines[3].Split(' ').Last());
				// BigInteger.TryParse(item.WorryLevel, out var worryLevel);
				int monkeyIndex = int.Parse(monkeyLines[(item.WorryLevel % divisor == 0) ? 4 : 5].Split(' ').Last());
				return monkeys[monkeyIndex];
			}

			monkeys.Add(new Monkey(monkeyNumber, initialItems, adjustWorryLevel, pickNextMonkey));
		}

		private Monkey(int number, IReadOnlyList<Item> initialItems, Func<Item, Item> adjustWorryLevel, Func<Item, Monkey> pickNextMonkey)
		{
			Number = number;
			m_items = new Queue<Item>(initialItems);
			m_adjustWorryLevel = adjustWorryLevel;
			m_pickNextMonkey = pickNextMonkey;
		}

		public int Number { get; }

		public int TotalItemsInspected { get; private set;}

		public void InspectAllItems()
		{
			while (m_items.TryDequeue(out Item item))
			{
				TotalItemsInspected++;
				item = m_adjustWorryLevel(item);
				var nextMonkey = m_pickNextMonkey(item);
				nextMonkey.m_items.Enqueue(item);
			}
		}

		private readonly Queue<Item> m_items;
		private readonly Func<Item, Item> m_adjustWorryLevel;
		private readonly Func<Item, Monkey> m_pickNextMonkey;
	}
}
