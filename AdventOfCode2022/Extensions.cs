namespace AdventOfCode2022
{
	internal static class Extensions
	{
		public static IEnumerable<(T Item, int Index)> WithIndexes<T>(this IEnumerable<T> items)
			=> items.Select((x, i) => (x, i));
	}
}