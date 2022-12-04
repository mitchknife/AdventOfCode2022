namespace AdventOfCode2022;

public class Day04 : IDay
{
	public IReadOnlyList<string> Execute(IReadOnlyList<string> input)
	{
		var assignmentPairs = input
			.Select(line => line.Split(','))
			.Select(split => new AssignmentPairs(getRange(split[0]), getRange(split[1])))
			.ToList();

		return new[]
		{
			assignmentPairs.Count(p => p.CommonSections.Count == p.Sections1.Count || p.CommonSections.Count == p.Sections2.Count).ToString(),
			assignmentPairs.Count(p => p.CommonSections.Any()).ToString(),
		};

		IReadOnlyList<int> getRange(string range)
		{
			var bookEnds = range.Split('-').Select(int.Parse).ToList();
			return Enumerable.Range(bookEnds[0], bookEnds[1] - bookEnds[0] + 1).ToList();
		} 
	}

	record AssignmentPairs(IReadOnlyList<int> Sections1, IReadOnlyList<int> Sections2)
	{
		public IReadOnlyList<int> CommonSections = Sections1.Intersect(Sections2).ToList();
	}
}
