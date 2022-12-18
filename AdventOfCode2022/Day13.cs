using System.Text.Json;
using System.Text.Json.Nodes;

namespace AdventOfCode2022;

public class Day13 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var packets = input
			.Where(x => x.Length > 0)
			.Select(x => new Packet(JsonSerializer.Deserialize<JsonArray>(x)))
			.ToList();
		
		yield return packets
			.Select((x, i) => (x, i))
			.GroupBy(x => x.i / 2, (x, items) => (num: x + 1, pair: items.Select(i => i.x).ToList()))
			.Where(x => x.pair[0].CompareTo(x.pair[1]) == -1)
			.Select(x => x.num)
			.Sum()
			.ToString();

		var dividerPacket1 = new Packet(new JsonArray(new JsonArray(JsonValue.Create(2))));
		var dividerPacket2 = new Packet(new JsonArray(new JsonArray(JsonValue.Create(6))));
		packets.Add(dividerPacket1);
		packets.Add(dividerPacket2);
		packets.Sort();

		yield return ((packets.IndexOf(dividerPacket1) + 1) * (packets.IndexOf(dividerPacket2) + 1)).ToString();
	}

	private record Packet(JsonArray JsonArray) : IComparable<Packet>
	{
		public int CompareTo(Packet other) => CompareNodes(JsonArray, other.JsonArray);

		private static int CompareNodes(JsonNode left, JsonNode right)
		{
			if (left is null)
				return -1;
			if (right is null)
				return 1;

			if (left is JsonValue && right is JsonValue)
			{
				int leftInt = left.GetValue<int>();
				int rightInt = right.GetValue<int>();
				return leftInt < rightInt ? -1 : rightInt < leftInt ? 1: 0;
			}
			
			if (left is JsonValue && right is JsonArray)
				return CompareNodes(new JsonArray(JsonValue.Create(left.GetValue<int>())), right);

			if (left is JsonArray && right is JsonValue)
				return CompareNodes(left, new JsonArray(JsonValue.Create(right.GetValue<int>())));

			if (left is JsonArray leftArray && right is JsonArray rightArray)
			{
				int maxLength = Math.Max(leftArray.Count, rightArray.Count);
				for (int i = 0; i < maxLength; i++)
				{
					var leftItem = leftArray.ElementAtOrDefault(i);
					var rightItem = rightArray.ElementAtOrDefault(i);

					var result = CompareNodes(leftItem, rightItem);
					if (result != 0)
						return result;
				}
				return 0;
			}
			
			throw new NotSupportedException();
		}
	}
}
