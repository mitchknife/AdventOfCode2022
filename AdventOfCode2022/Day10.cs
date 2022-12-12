namespace AdventOfCode2022;

public class Day10 : IDay
{
	public IReadOnlyList<string> Execute(IReadOnlyList<string> input)
	{
		var output = new List<string>();
		var allCycles = Run(input).ToList();
		output.Add(allCycles.Where(x => x.Cycle == 20 || (x.Cycle - 20) % 40 == 0).Take(6).Select(x => x.X * x.Cycle).Sum().ToString());

		foreach (var row in allCycles.GroupBy(x => (x.Cycle - 1) / 40))
		{
			string line = "";
			foreach (var (x, pos) in row.Select((x, i) => (x.X, i)))
				line += (pos == x || pos + 1 == x || pos - 1 == x) ? "#" : ".";
			output.Add(line);
		}

		return output;
	}

	private static IEnumerable<(int Cycle, int X)> Run(IEnumerable<string> instuctions)
	{
		int x = 1;
		int cycle = 0;
		foreach (var instruction in instuctions)
		{
			cycle++;
			yield return (cycle, x);

			var tokens = instruction.Split(' ');
			if (tokens[0] == "addx")
			{
				cycle++;
				yield return (cycle, x);
				x += int.Parse(tokens[1]);
			}
		}
	}
}
