namespace AdventOfCode2022;

public class Day10 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var output = new List<string>();
		yield return Run(input).Where(x => x.Cycle == 20 || (x.Cycle - 20) % 40 == 0).Take(6).Sum(x => x.X * x.Cycle).ToString();

		foreach (var row in Run(input).GroupBy(x => (x.Cycle - 1) / 40))
		{
			string line = "";
			foreach (var (x, pos) in row.Select((x, i) => (x.X, i)))
				line += (pos == x || pos + 1 == x || pos - 1 == x) ? "#" : ".";

			yield return line;
		}
	}

	private static IEnumerable<(int Cycle, int X)> Run(IEnumerable<string> instuctions)
	{
		int x = 1;
		int cycle = 0;
		foreach (var instruction in instuctions)
		{
			yield return (++cycle, x);

			var tokens = instruction.Split(' ');
			if (tokens[0] == "addx")
			{
				yield return (++cycle, x);
				x += int.Parse(tokens[1]);
			}
		}
	}
}
