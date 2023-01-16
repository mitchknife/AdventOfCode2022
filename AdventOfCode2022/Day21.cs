namespace AdventOfCode2022;

public class Day21 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var monkeys = new Dictionary<string, Monkey>();
		foreach (var tokens in input.Select(x => x.Split(' ')))
		{
			var monkey = new Monkey(tokens[0].Trim(':'));
			if (tokens.Length == 2)
				monkey.SetValue(long.Parse(tokens[1]));
			else
				monkey.SetValue(new GetValueOperation(new(() => monkeys[tokens[1]]), new(() => monkeys[tokens[3]]), tokens[2]));

			monkeys.Add(monkey.Name, monkey);
		}

		var root = monkeys["root"];
		yield return root.GetValue().ToString();

		var humn = monkeys["humn"];
		humn.SetValue((int?) null);
		
		if (root.Op.LeftMonkey.Value.GetValue() is null)
			root.Op.LeftMonkey.Value.SetValue(root.Op.RightMonkey.Value.GetValue());
		else
			root.Op.RightMonkey.Value.SetValue(root.Op.LeftMonkey.Value.GetValue());

		yield return humn.GetValue().ToString();
	}

	private record Monkey(string Name)
	{
		public GetValueOperation Op { get; private set;}

		public long? GetValue()
		{
			if (Op is null)
				return m_value;

			long? leftValue = Op.LeftMonkey.Value.GetValue();
			long? rightValue = Op.RightMonkey.Value.GetValue();
			if (leftValue is null || rightValue is null)
				return null;

			return Op.Op switch
			{
				"+" => leftValue + rightValue,
				"-" => leftValue - rightValue,
				"*" => leftValue * rightValue,
				"/" => leftValue / rightValue,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		public void SetValue(long? value)
		{
			if (Op is null)
			{
				m_value = value;
				return;
			}
		
			if (value is null)
				throw new NotSupportedException();

			long? leftMonkeyValue = Op.LeftMonkey.Value.GetValue();
			long? rightMonkeyValue = Op.RightMonkey.Value.GetValue();

			if (leftMonkeyValue is not null)
			{
				Op.RightMonkey.Value.SetValue(Op.Op switch
				{
					"+" => value.Value - leftMonkeyValue.Value,
					"-" => leftMonkeyValue.Value - value.Value,
					"*" => value.Value / leftMonkeyValue.Value,
					"/" => leftMonkeyValue.Value / value.Value,
					_ => throw new ArgumentOutOfRangeException(),
				});
			}
			else
			{
				Op.LeftMonkey.Value.SetValue(Op.Op switch
				{
					"+" => value.Value - rightMonkeyValue.Value,
					"-" => value.Value + rightMonkeyValue.Value,
					"*" => value.Value / rightMonkeyValue.Value,
					"/" => value.Value * rightMonkeyValue.Value,
					_ => throw new ArgumentOutOfRangeException(),
				});
			}
		}

		public void SetValue(GetValueOperation operation) => Op = operation;

		private long? m_value;
	}

	private record GetValueOperation(Lazy<Monkey> LeftMonkey, Lazy<Monkey> RightMonkey, string Op);
}