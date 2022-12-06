using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2022.Tests;

public class DayTests
{
	[Theory]
	[DayTestCases(1)]
	[DayTestCases(2)]
	[DayTestCases(3)]
	[DayTestCases(4)]
	[DayTestCases(5)]
	[DayTestCases(6)]
	public void AllDays(IDay day, string input, string expectedOutput)
	{
		var inputLines = input.Split(Environment.NewLine);
		var expectedOutputLines = expectedOutput.Split(Environment.NewLine);
		day.Execute(inputLines).Should().Equal(expectedOutputLines);
	}
}

public class DayTestCasesAttribute : DataAttribute
{
	public DayTestCasesAttribute(int day)
	{
		m_day = day;
	}

	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		string dayTypeName = $"Day{m_day:D2}";

		string testCasesFolder = Path.Combine(Environment.CurrentDirectory, $"TestCases/{dayTypeName}");
		if (!Directory.Exists(testCasesFolder))
			throw new InvalidOperationException($"Folder does not exist: {testCasesFolder}");

		var dayType = typeof(IDay).Assembly.GetTypes().FirstOrDefault(x => x.Name == dayTypeName);
		if (dayType is null)
			throw new InvalidOperationException($"Type does not exist: {dayTypeName}");

		var day = (IDay) Activator.CreateInstance(dayType);
		foreach (var files in Directory.GetFiles(testCasesFolder, "*.txt").GroupBy(file => Path.GetFileName(file).Split('_')[0]))
		{
			string inFileName = $"{files.Key}_in.txt";
			string inFile = files.FirstOrDefault(x => Path.GetFileName(x).ToLower() == inFileName);
			if (inFile is null)
				throw new InvalidOperationException($"File does not exist: {inFileName}");

			string outFileName = $"{files.Key}_out.txt";
			string outFile = files.FirstOrDefault(x => Path.GetFileName(x.ToLower()) == outFileName);
			if (outFile is null)
				throw new InvalidOperationException($"File does not exist: {outFileName}");

			yield return new object[] { day, File.ReadAllText(inFile).TrimEnd(), File.ReadAllText(outFile).TrimEnd() };
		}
	}

	private readonly int m_day;
}