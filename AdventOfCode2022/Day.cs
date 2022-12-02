namespace AdventOfCode2022;

public abstract class Day
{
	public string Execute(string input)
	{
		var output = ExecuteCore(input.Split(Environment.NewLine));
		return string.Join(Environment.NewLine, output.Append(""));
	}

	public abstract IReadOnlyList<string> ExecuteCore(IReadOnlyList<string> input);
}