namespace AdventOfCode2022;

public class Day07 : IDay
{
	public IReadOnlyList<string> Execute(IReadOnlyList<string> input)
	{
		var baseDir = new Dir("/", null);
		var currentDir = baseDir;
		foreach (string line in input.Skip(1))
		{
			var tokens = line.Split(' ');
			if (tokens[0] == "$" && tokens[1] == "cd")
				currentDir = tokens[2] == ".." ? currentDir.Parent : currentDir.GetSubDir(tokens[2]);
			else if (tokens[0] == "dir")
				currentDir.AddSubDir(new Dir(tokens[1], currentDir));
			else if (int.TryParse(tokens[0], out int fileSize))
				currentDir.AddFileSize(fileSize);
		}

		int needToFreeSize = 30000000 - (70000000 - baseDir.TotalSize);
		var dirSizes = baseDir.EnumerateAllSubDirs().Select(x => x.TotalSize).ToList();

		return new[]
		{
			dirSizes.Where(x => x <= 100000).Sum().ToString(),
			dirSizes.Where(x => x >= needToFreeSize).OrderBy(x => x).First().ToString(),
		};
	}

	record Dir(string Name, Dir Parent)
	{
		public int TotalSize => m_fileSizes.Sum() + m_subDirs.Sum(x => x.TotalSize);
		public void AddSubDir(Dir subDir) => m_subDirs.Add(subDir);
		public Dir GetSubDir(string name) => m_subDirs.Single(x => x.Name == name);
		public void AddFileSize(int fileSize) => m_fileSizes.Add(fileSize);

		public IEnumerable<Dir> EnumerateAllSubDirs()
		{
			foreach (var subDir in m_subDirs)
			{
				yield return subDir;
				foreach (var subSubDir in subDir.EnumerateAllSubDirs())
					yield return subSubDir;
			}
		}

		readonly List<Dir> m_subDirs = new List<Dir>();
		readonly List<int> m_fileSizes = new List<int>();
	}
}