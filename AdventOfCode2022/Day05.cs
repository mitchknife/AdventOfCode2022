namespace AdventOfCode2022;

public class Day05 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		int indexOfEmptyLine = input.ToList().FindIndex(string.IsNullOrEmpty);
		List<Stack<char>> part1Stacks = null;

		foreach (string line in input.Take(indexOfEmptyLine).Reverse())
		{
			var lineItems = line
				.Select((str, index) => (str, index))
				.GroupBy(x => x.index / 4)
				.Select(g => g.Select(x => x.str).Skip(1).First())
				.ToList();

			if (part1Stacks is null)
			{
				part1Stacks = lineItems.Select(x => new Stack<char>()).ToList();
			}
			else
			{
				foreach (var (stack, item) in part1Stacks.Zip(lineItems).Where(x => x.Second != ' '))
					stack.Push(item);
			}
		}

		var part2Stacks = part1Stacks.Select(x => new Stack<char>(x.Reverse())).ToList();
		foreach (string line in input.Skip(indexOfEmptyLine + 1))
		{
			var ints = line.Split(' ')
				.Where(x => int.TryParse(x, out int _))
				.Select(int.Parse)
				.ToList();

			var moves = Enumerable.Range(0, ints[0]);
			int fromStackIndex = ints[1] - 1;
			int toStackIndex = ints[2] - 1;

			foreach (var _ in moves)
				part1Stacks[toStackIndex].Push(part1Stacks[fromStackIndex].Pop());

			foreach (var item in moves.Select(_ => part2Stacks[fromStackIndex].Pop()).Reverse())
				part2Stacks[toStackIndex].Push(item);
		}

		yield return string.Join("", part1Stacks.Select(x => x.Peek()));
		yield return string.Join("", part2Stacks.Select(x => x.Peek()));
	}
}
