namespace AdventOfCode2022;

public class Day18 : IDay
{
	public IEnumerable<string> Execute(IReadOnlyList<string> input)
	{
		var cubes = input
			.Select(x => x.Split(','))
			.Select(tokens => new Cube(new Vector(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]))))
			.ToHashSet();

		yield return GetExposedSides(cubes).Count.ToString();
		yield return GetExposedSides(cubes, onlySurfaceSides: true).Count.ToString();
	}

	private IReadOnlyList<Side> GetExposedSides(HashSet<Cube> cubes, bool onlySurfaceSides = false)
	{
		var min = new Vector(int.MaxValue, int.MaxValue, int.MaxValue);
		var max = new Vector(int.MinValue, int.MinValue, int.MinValue);
		var exposedSides = new HashSet<Side>();
		var corners = new HashSet<Vector>();
		foreach (var cube in cubes)
		{
			foreach (var side in cube.GetSides())
			{
				if (!exposedSides.Add(side))
					exposedSides.Remove(side);

				min = new Vector(Math.Min(min.X, cube.Location.X), Math.Min(min.Y, cube.Location.Y), Math.Min(min.Z, cube.Location.Z));
				max = new Vector(Math.Max(max.X, cube.Location.X), Math.Max(max.Y, cube.Location.Y), Math.Max(max.Z, cube.Location.Z));
				corners.UnionWith(cube.GetCorners());
			}
		}

		if (onlySurfaceSides)
		{
			var outerSurfaceMin = min - 1;
			var outerSurfaceMax = max + 1;
			var outerSurfaceCubes = new HashSet<Cube>();
			addOuterSurfaceCubes(new Cube(cubes.First(x => x.Location.X == min.X).Location - 1));

			bool isOuterSurfaceCube(Cube cube) =>
				!cubes.Contains(cube) &&
				cube.GetCorners().Any(corners.Contains) &&
				cube.Location.X >= outerSurfaceMin.X && cube.Location.X <= outerSurfaceMax.X &&
				cube.Location.Y >= outerSurfaceMin.Y && cube.Location.Y <= outerSurfaceMax.Y &&
				cube.Location.Z >= outerSurfaceMin.Z && cube.Location.Z <= outerSurfaceMax.Z;

			void addOuterSurfaceCubes(Cube cube)
			{
				if (isOuterSurfaceCube(cube) && outerSurfaceCubes.Add(cube))
				{
					foreach (var adjacentEmptyCube in cube.GetAdjacentCubes())
						addOuterSurfaceCubes(adjacentEmptyCube);
				}
			}

			exposedSides.IntersectWith(outerSurfaceCubes.SelectMany(x => x.GetSides()));
		}

		return exposedSides.ToList();
	}

	private record Cube(Vector Location)
	{
		public IEnumerable<Side> GetSides()
		{
			yield return new(Location - (0, 0, 1), Plane.Z);
			yield return new(Location, Plane.Z);
			yield return new(Location - (0, 1, 0), Plane.Y);
			yield return new(Location, Plane.Y);
			yield return new(Location - (1, 0, 0), Plane.X);
			yield return new(Location, Plane.X);
		}

		public IEnumerable<Cube> GetAdjacentCubes()
		{
			yield return new Cube(Location + (1, 0, 0));
			yield return new Cube(Location - (1, 0, 0));
			yield return new Cube(Location + (0, 1, 0));
			yield return new Cube(Location - (0, 1, 0));
			yield return new Cube(Location + (0, 0, 1));
			yield return new Cube(Location - (0, 0, 1));
		}

		public IEnumerable<Vector> GetCorners()
		{
			yield return Location - (0, 0, 0);
			yield return Location - (0, 0, 1);
			yield return Location - (0, 1, 0);
			yield return Location - (0, 1, 1);
			yield return Location - (1, 0, 0);
			yield return Location - (1, 0, 1);
			yield return Location - (1, 1, 0);
			yield return Location - (1, 1, 1);
		}
	}

	private record Side(Vector Location, Plane Plane);
	private enum Plane { X, Y, Z };
}