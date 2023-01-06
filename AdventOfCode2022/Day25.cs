namespace AdventOfCode2022;

public class Day25 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> snafus)
	{
		long base10Sum = snafus.Select(ToBase10).Sum();
		yield return ToSnafu(base10Sum);
	}

	public long ToBase10(string snafu) => snafu
		.Reverse()
		.Select((ch, i) => (long) Math.Pow(5, i) * ch switch
		{
			'=' => -2,
			'-' => -1,
			_ => int.Parse(ch.ToString()),
		})
		.Sum();

	public static string ToSnafu(long base10)
	{
		if (base10 is 0)
			return "0";

		var base5Digits = Enumerable.Range(0, int.MaxValue)
			.Select(x => (long) Math.Pow(5, x))
			.TakeWhile(placeValue => base10 >= placeValue)
			.Reverse()
			.Select(placeValue => (int) Math.DivRem(base10, placeValue, out base10))
			.Reverse()
			.ToList();

		var snafuDigits = new List<int>();
		int carry = 0;
		foreach (var base5Digit in base5Digits)
		{
			int snafuDigit = base5Digit + carry;
			if (snafuDigit is 3 or 4 or 5)
			{
				snafuDigit -= 5;
				carry = 1;
			}
			else
			{
				carry = 0;
			}

			snafuDigits.Add(snafuDigit);
		}

		if (carry > 0)
			snafuDigits.Add(1);

		return string.Join("", snafuDigits.Reverse<int>().Select(x => x switch
		{
			-2 => "=",
			-1 => "-",
			_ => x.ToString(),
		}));
	}
}
