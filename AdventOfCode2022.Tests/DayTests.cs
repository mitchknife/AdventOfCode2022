using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2022.Tests;

public class DayTests
{
	[Theory]
	[TestDay(1)]
	public void AllDays(Day day, string input, string expected)
	{
		day.Execute(input).Should().Be(expected);
	}
}

public class TestDayAttribute : DataAttribute
{
	public TestDayAttribute(int day)
	{
		m_day = day;
	}

	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		string dayTypeName = $"Day{m_day}";

		string testCasesFolder = Path.Combine(Environment.CurrentDirectory, $"TestCases/{dayTypeName}");
		if (!Directory.Exists(testCasesFolder))
			throw new InvalidOperationException($"Folder does not exist: {testCasesFolder}");

		var dayType = typeof(Day).Assembly.GetTypes().FirstOrDefault(x => x.Name == dayTypeName);
		if (dayType is null)
			throw new InvalidOperationException($"Type does not exist: {dayTypeName}");

		var day = (Day) Activator.CreateInstance(dayType);
		foreach (var files in Directory.GetFiles(testCasesFolder, "*.txt").GroupBy(file => Path.GetFileName(file).Split('_')[0]))
		{
			string inFileName = $"{files.Key}_in.txt";
			string inFile = files.FirstOrDefault(x => Path.GetFileName(x).ToLower() == inFileName);
			if (inFile is null)
				throw new InvalidOperationException($"Test input file missing: {inFileName}");

			string outFileName = $"{files.Key}_out.txt";
			string outFile = files.FirstOrDefault(x => Path.GetFileName(x.ToLower()) == outFileName);
			if (outFile is null)
				throw new InvalidOperationException($"Expected output file missing: {outFileName}");

			yield return new object[] { day, File.ReadAllText(inFile), File.ReadAllText(outFile) };
		}
	}

	private readonly int m_day;
}