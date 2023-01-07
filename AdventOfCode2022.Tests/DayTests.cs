using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace AdventOfCode2022.Tests;

public class DayTests
{
	[Theory]
	[DayTestCases(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20, 25)]
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

	public static bool RunSlowTests = bool.Parse(Environment.GetEnvironmentVariable(nameof(RunSlowTests)) ?? "false");

	public override IEnumerable<object[]> GetData(MethodInfo testMethod)
	{
		var notImplementedTests = new List<string>();
		var slowTests = new List<string>();
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

				if (input.StartsWith("slow test"))
				{
					if (!RunSlowTests)
					{
						slowTests.Add($"{dayType.Name}/{inFileName}");
						continue;
					}

					input = input.Substring(9).TrimStart();
				}

				yield return new object[] { day, input, output };
			}
		}

		if (notImplementedTests.Count > 0)
			Console.WriteLine($"Skipping not implemented tests: {string.Join(", ", notImplementedTests)}");
		if (slowTests.Count > 0)
			Console.WriteLine($"Skipping slow tests: {string.Join(", ", slowTests)}");
	}

	private readonly IReadOnlyList<int> m_dayNumbers;
}