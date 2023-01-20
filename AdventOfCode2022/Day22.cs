namespace AdventOfCode2022;

public class Day22 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var board = Board.Create(input.TakeWhile(line => line.Length > 0).ToList());
		yield return board.GetPasswordFromPath(input.Last()).ToString();
		yield return board.GetPasswordFromPath(input.Last(), foldAsCube: true).ToString();
	}

	private class Board
	{
		public static Board Create(IReadOnlyList<string> boardLines)
		{
			int rowCount = boardLines.Count();
			int columnCount = boardLines.Select(x => x.Length).Max();
			int sideLength = Math.Max(rowCount, columnCount) / 4;
			int side = 1;
			var tiles = new List<Tile>();
			
			foreach (var (rowMin, firstRow) in Enumerable.Range(1, 4)
				.Select(x => sideLength * x - sideLength + 1)
				.Select(rowMin => (rowMin, firstRow: boardLines.ElementAtOrDefault(rowMin - 1))))
			{
				foreach (int colMin in Enumerable.Range(1, 4)
					.Select(x => sideLength * x - sideLength + 1)
					.Where(colMin => firstRow?.ElementAtOrDefault(colMin - 1) is '.' or '#'))
				{
					foreach (var (boardRow, sideRow) in Enumerable.Range(rowMin, sideLength)
						.Select((x, i) => (x, i + 1)))
					{
						foreach (var (boardCol, sideCol) in Enumerable.Range(colMin, sideLength)
							.Select((x, i) => (x, i + 1)))
						{
							tiles.Add(new Tile(
								Location: (boardCol, boardRow),
								SideLocation: (sideCol, sideRow),
								IsWall: boardLines[boardRow - 1][boardCol - 1] is '#',
								Side: side));
						}
					}

					side++;
				}
			}

			return new Board(tiles, sideLength);
		}

		public int GetPasswordFromPath(string path, bool foldAsCube = false)
		{
			string toMoveString = "";
			var direction = Direction.Right;
			var tile = m_tiles.Values
				.Where(x => !x.IsWall)
				.OrderBy(x => x.Location.Y)
				.ThenBy(x => x.Location.X)
				.First();

			foreach (char ch in path.Append('X'))
			{
				if (char.IsDigit(ch))
				{
					toMoveString += ch;
				}
				else
				{
					foreach (int _ in Enumerable.Range(0, int.Parse(toMoveString)))
					{
						var (nextTile, nextDirection) = GetNextTileAndDirection(tile, direction, foldAsCube);
						if (nextTile.IsWall)
							break;

						tile = nextTile;
						direction = nextDirection;
					}

					toMoveString = "";
					if (ch is 'R')
						direction = direction.Rotate(1);
					else if (ch is 'L')
						direction = direction.Rotate(-1);
				}
			}

			return 1000 * tile.Location.Y + 4 * tile.Location.X + direction switch
			{
				Direction.Right => 0,
				Direction.Down => 1 ,
				Direction.Left => 2 ,
				Direction.Up => 3,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		private (Tile, Direction) GetNextTileAndDirection(Tile tile, Direction direction, bool isCube)
		{
			var nextTile = m_tiles.GetValueOrDefault(tile.Location.Move(direction));
			var nextDirection = direction;

			if (nextTile is null)
			{
				if (isCube)
				{
					var mapping = (m_sideLength == 4 ? s_exampleSideMappings : s_actualSideMappings)[(tile.Side, direction)];
					var translatedSideLocation = m_sideTranslations[(direction, mapping.Direction)][tile.SideLocation];
					nextTile = m_tilesBySideLocation[(mapping.Side, translatedSideLocation)];
					nextDirection = mapping.Direction.Rotate(2);
				}
				else
				{
					nextTile = direction switch
					{
						Direction.Up => m_tiles.Values.Where(t => t.Location.X == tile.Location.X).MaxBy(t => t.Location.Y),
						Direction.Right => m_tiles.Values.Where(t => t.Location.Y == tile.Location.Y).MinBy(t => t.Location.X),
						Direction.Down => m_tiles.Values.Where(t => t.Location.X == tile.Location.X).MinBy(t => t.Location.Y),
						Direction.Left => m_tiles.Values.Where(t => t.Location.Y == tile.Location.Y).MaxBy(t => t.Location.X),
						_ => throw new ArgumentOutOfRangeException(),
					};
				}
			}

			return (nextTile, nextDirection);
		}

		private Board(IReadOnlyList<Tile> tiles, int sideLength)
		{
			m_tiles = tiles.ToDictionary(x => x.Location);
			m_tilesBySideLocation = tiles.ToDictionary(x => (x.Side, x.SideLocation));
			m_sideTranslations = CreateSideTranslations(sideLength);
			m_sideLength = sideLength;
		}

		private static IReadOnlyDictionary<(Direction, Direction), Dictionary<Vector2D, Vector2D>> CreateSideTranslations(int sideLength)
		{
			var translations = new Dictionary<(Direction, Direction), Dictionary<Vector2D, Vector2D>>();
			foreach (var from in Enum.GetValues<Direction>())
			{
				foreach (var to in Enum.GetValues<Direction>())
				{
					var fromLocations = getSideLocations(from);
					var toLocations = getSideLocations(to);

					bool shouldReverse = from == to ||
						(from is Direction.Up && to is Direction.Right) ||
						(from is Direction.Right && to is Direction.Up) ||
						(from is Direction.Down && to is Direction.Left) ||
						(from is Direction.Left && to is Direction.Down);

					if (shouldReverse)
						toLocations = toLocations.Reverse();
					
					translations.Add((from, to), fromLocations.Zip(toLocations).ToDictionary(x => x.First, x => x.Second));
				}
			}
			return translations;

			IEnumerable<Vector2D> getSideLocations(Direction direction) => direction switch
			{
				Direction.Up => Enumerable.Range(1, sideLength).Select(i => new Vector2D(i, 1)),
				Direction.Right => Enumerable.Range(1, sideLength).Select(i => new Vector2D(sideLength, i)),
				Direction.Down => Enumerable.Range(1, sideLength).Select(i => new Vector2D(i, sideLength)),
				Direction.Left => Enumerable.Range(1, sideLength).Select(i => new Vector2D(1, i)),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		private static readonly IReadOnlyDictionary<(int Side, Direction Direction), (int Side, Direction Direction)> s_exampleSideMappings = new Dictionary<(int, Direction), (int, Direction)>
		{
			[(1, Direction.Up)] = (2, Direction.Up),
			[(1, Direction.Right)] = (6, Direction.Right),
			[(1, Direction.Left)] = (3, Direction.Up),

			[(2, Direction.Up)] = (1, Direction.Up),
			[(2, Direction.Down)] = (5, Direction.Down),
			[(2, Direction.Left)] = (6, Direction.Down),

			[(3, Direction.Up)] = (1, Direction.Left),
			[(3, Direction.Down)] = (5, Direction.Left),
			
			[(4, Direction.Right)] = (6, Direction.Up),

			[(5, Direction.Down)] = (2, Direction.Down),
			[(5, Direction.Left)] = (3, Direction.Down),

			[(6, Direction.Up)] = (4, Direction.Right),
			[(6, Direction.Right)] = (1, Direction.Right),
			[(6, Direction.Down)] = (2, Direction.Left),
		};

		private static readonly IReadOnlyDictionary<(int Side, Direction Direction), (int Side, Direction Direction)> s_actualSideMappings = new Dictionary<(int, Direction), (int, Direction)>
		{
			[(1, Direction.Up)] = (6, Direction.Left),
			[(1, Direction.Left)] = (4, Direction.Left),

			[(2, Direction.Up)] = (6, Direction.Down),
			[(2, Direction.Right)] = (5, Direction.Right),
			[(2, Direction.Down)] = (3, Direction.Right),

			[(3, Direction.Right)] = (2, Direction.Down),
			[(3, Direction.Left)] = (4, Direction.Up),
			
			[(4, Direction.Up)] = (3, Direction.Left),
			[(4, Direction.Left)] = (1, Direction.Left),

			[(5, Direction.Right)] = (2, Direction.Right),
			[(5, Direction.Down)] = (6, Direction.Right),

			[(6, Direction.Right)] = (5, Direction.Down),
			[(6, Direction.Down)] = (2, Direction.Up),
			[(6, Direction.Left)] = (1, Direction.Up),
		};

		private readonly IReadOnlyDictionary<Vector2D, Tile> m_tiles;
		private readonly IReadOnlyDictionary<(int, Vector2D), Tile> m_tilesBySideLocation;
		private readonly IReadOnlyDictionary<(Direction, Direction), Dictionary<Vector2D, Vector2D>> m_sideTranslations;
		private readonly int m_sideLength;
	}

	private record Tile(Vector2D Location, Vector2D SideLocation, bool IsWall, int Side);
}
