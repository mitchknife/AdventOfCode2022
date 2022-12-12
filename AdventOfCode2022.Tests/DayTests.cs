using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2022.Tests;

public class DayTests
{
	[Theory]
	[DayTestCases(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
	public void AllDays(IDay day, string input, string expectedOutput)
	{
		var inputLines = input.Split(Environment.NewLine);
		var expectedOutputLines = expectedOutput.Split(Environment.NewLine);
		day.Execute(inputLines).Should().Equal(expectedOutputLines);
	}
}

public class DayTestCasesAttribute : DataAttribute
{
	public DayTestCasesAttribute(params int[] days)
	{
		m_dayNumbers = days;
	}

	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		foreach (int dayNumber in m_dayNumbers)
		{
			string dayTypeName = $"Day{dayNumber:D2}";
			var dayType = typeof(IDay).Assembly.GetTypes().FirstOrDefault(x => x.Name == dayTypeName);
			if (dayType is null)
				throw new InvalidOperationException($"Type does not exist: {dayTypeName}");

			string testCasesFolder = Path.Combine(Environment.CurrentDirectory, $"TestCases/{dayType.Name}");
			if (!Directory.Exists(testCasesFolder))
				throw new InvalidOperationException($"Folder does not exist: {testCasesFolder}");

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
	}

	private readonly IReadOnlyList<int> m_dayNumbers;
}