using System.Diagnostics;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2022.Tests;

public class DayTests
{
	[Theory]
	[DayTestCases(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25)]
	public void AllDays(IDay day, string input, string expectedOutput)
	{
		string testName = day.GetType().Name;
		bool isFirstTest = m_testNames.Add(testName);
		if (isFirstTest)
		{
			m_testStopwatch = Stopwatch.StartNew();
			Console.Write($"{testName}...");
		}

		var inputLines = input.Split(Environment.NewLine);
		var expectedOutputLines = expectedOutput.Split(Environment.NewLine);
		day.Execute(inputLines).Should().Equal(expectedOutputLines);

		if (!isFirstTest)
		{
			m_testStopwatch.Stop();
			Console.WriteLine($"done ({m_testStopwatch.ElapsedMilliseconds}ms)");
		}
	}

	private static Stopwatch m_testStopwatch;
	private static readonly HashSet<string> m_testNames = new HashSet<string>();
}

public class DayTestCasesAttribute : DataAttribute
{
	public DayTestCasesAttribute(params int[] days)
	{
		m_dayNumbers = days;
	}

	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		var notImplementedTests = new List<string>();
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

				string input = File.ReadAllText(inFile).TrimEnd();
				string output = File.ReadAllText(outFile).TrimEnd();
				if (input.StartsWith("not implemented"))
				{
					notImplementedTests.Add($"{dayType.Name}/{inFileName}");
					continue;
				}

				yield return new object[] { day, input, output };
			}
		}

		if (notImplementedTests.Count > 0)
			Console.WriteLine($"Skipping not implemented tests: {string.Join(", ", notImplementedTests)}");
	}

	private readonly IReadOnlyList<int> m_dayNumbers;
}