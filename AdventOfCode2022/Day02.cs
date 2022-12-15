namespace AdventOfCode2022;

public class Day02 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var rounds = new List<Round>();
		foreach (string line in input.Where(x => x.Length > 0))
		{
			var lineSplit = line.Split(' ');

			rounds.Add(new Round(
				Them: s_actions.Single(x => x.TheirSymbol == lineSplit[0]),
				Me: s_actions.Single(x => x.MySymbol == lineSplit[1])));
		}

		yield return rounds.Sum(x => x.CalculateFirstScore()).ToString();
		yield return rounds.Sum(x => x.CalculateSecondScore()).ToString();
	}

	private static IReadOnlyList<Action> s_actions = new Action[]
	{
		new("A", "X", 1),
		new("B", "Y", 2),
		new("C", "Z", 3),
	};

	private record Action(string TheirSymbol, string MySymbol, int Value);

	private record Round(Action Them, Action Me)
	{
		public int CalculateFirstScore()
		{
			int score = 
				Me == Them ? 3 :
				Me == s_actions.First() && Them == s_actions.Last() ? 6 :
				Me == s_actions.Last() && Them == s_actions.First() ? 0 :
				Me.Value > Them.Value ? 6 :
				0;

			return score + Me.Value;
		}

		public int CalculateSecondScore() => Me.MySymbol switch
		{
			"X" when Them == s_actions.First() => 0 + s_actions.Last().Value,
			"X" => 0 + Them.Value - 1,
			"Y" => 3 + Them.Value,
			"Z" when Them == s_actions.Last() => 6 + s_actions.First().Value,
			"Z" => 6 + Them.Value + 1,
			_ => throw new ArgumentOutOfRangeException(Me.MySymbol),
		};
	}
}