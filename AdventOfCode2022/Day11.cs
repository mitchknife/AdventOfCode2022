namespace AdventOfCode2022;

public class Day11 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		yield return GetLevelOfMonkeyBusiness(input, 20, 3).ToString();
		yield return GetLevelOfMonkeyBusiness(input, 10000, null).ToString();
	}

	private long GetLevelOfMonkeyBusiness(IReadOnlyList<string> input, int rounds, int? worryModifier)
	{
		var monkeys = input
			.Select((x, i) => (x, i))
			.GroupBy(x => x.i / 7, x => x.x, (key, items) => items.ToList())
			.Select(Monkey.Create)
			.ToList();

		foreach (var round in Enumerable.Range(1, rounds))
		{
			foreach (var monkey in monkeys)
				monkey.InspectItems(monkeys, worryModifier);
		}

		return monkeys
			.OrderByDescending(x => x.TotalItemsInspected)
			.Take(2)
			.Select(x => (long) x.TotalItemsInspected)
			.Aggregate(1L, (x, y) => x * y);
	}

	private record Item(long WorryLevel);
	private record Operation(string Op, int? Value);
	private record Test(int Divisor, int TrueMonkeyId, int FalseMonkeyId);

	private record Monkey(int Id, Queue<Item> Items, Operation Operation, Test Test)
	{
		public static Monkey Create(IReadOnlyList<string> monkeyLines)
		{
			var operationTokens = monkeyLines[2].Split('=')[1].Split(' ');
			return new Monkey(
				Id: int.Parse(monkeyLines[0].Split(' ').Last().TrimEnd(':')),
				Items: new(monkeyLines[1].Split(':')[1].Split(',').Select(x => new Item(long.Parse(x.Trim())))),
				Operation: new (
					Op: operationTokens[2],
					Value: int.TryParse(operationTokens[3], out var i) ? i : null
				),
				Test: new (
					Divisor: int.Parse(monkeyLines[3].Split(' ').Last()),
					TrueMonkeyId: int.Parse(monkeyLines[4].Split(' ').Last()),
					FalseMonkeyId: int.Parse(monkeyLines[5].Split(' ').Last())
				)
			);
		}

		public int TotalItemsInspected { get; private set; }

		public void InspectItems(IReadOnlyList<Monkey> monkeys, int? worryModifier)
		{
			int mod = monkeys.Select(x => x.Test.Divisor).Aggregate(1, (x, y) => x * y);
			while (Items.TryDequeue(out Item item))
			{
				long worryLevel = item.WorryLevel;
				if (worryModifier is null)
					worryLevel = worryLevel % mod;
				
				long opValue = Operation.Value ?? worryLevel;
				worryLevel = Operation.Op == "*"
					? worryLevel * opValue
					: worryLevel + opValue;

				if (worryModifier is not null)
					worryLevel = worryLevel / worryModifier.Value;

				var nextMonkeyId = worryLevel % Test.Divisor == 0 ? Test.TrueMonkeyId : Test.FalseMonkeyId;
				monkeys.Single(x => x.Id == nextMonkeyId).Items.Enqueue(new Item(worryLevel));
				TotalItemsInspected++;
			}
		}
	}
}
