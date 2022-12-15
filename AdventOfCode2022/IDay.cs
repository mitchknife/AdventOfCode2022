namespace AdventOfCode2022;

public interface IDay
{
	IEnumerable<string> Execute(IReadOnlyList<string> input);
}
