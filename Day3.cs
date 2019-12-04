using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;


public static class Day3 {

    public static string Problem1() {
        Debug.Assert(
            ClosestIntersection(
                new Path(new[] { "R8","U5","L5","D3" }), 
                new Path(new[] { "U7", "R6", "D4", "L4" })
            )
            == 6
        );

        Debug.Assert(
            ClosestIntersection(
                new Path(new[] { "R75","D30","R83","U83","L12","D49","R71","U7","L72" }), 
                new Path(new[] { "U62","R66","U55","R34","D71","R55","D58","R83" })
            )
            == 159
        );

        Debug.Assert(
            ClosestIntersection(
                new Path(new[] { "R98","U47","R26","D63","R33","U87","L62","D20","R33","U53","R51" }), 
                new Path(new[] { "U98","R91","D20","R16","D67","R40","U7","R15","U6","R7" })
            )
            == 135
        );

        var input = File.ReadAllLines(".\\data\\day3.txt").Select(l => l.Split(',')).ToArray();
        return ClosestIntersection(new Path(input[0]), new Path(input[1])).ToString();
    }

    public static string Problem2() {
        
        var a610 = ShortestStepPathIntersection(
            new Path(new[] { "R75","D30","R83","U83","L12","D49","R71","U7","L72" }), 
            new Path(new[] { "U62","R66","U55","R34","D71","R55","D58","R83" })
        );
        Console.WriteLine(a610);

        Debug.Assert(
            ShortestStepPathIntersection(
                new Path(new[] { "R75","D30","R83","U83","L12","D49","R71","U7","L72" }), 
                new Path(new[] { "U62","R66","U55","R34","D71","R55","D58","R83" })
            )
            == 610
        );

        Debug.Assert(
            ShortestStepPathIntersection(
                new Path(new[] { "R98","U47","R26","D63","R33","U87","L62","D20","R33","U53","R51" }), 
                new Path(new[] { "U98","R91","D20","R16","D67","R40","U7","R15","U6","R7" })
            )
            == 410
        );

        var start = DateTimeOffset.Now;
        var input = File.ReadAllLines(".\\data\\day3.txt").Select(l => l.Split(',')).ToArray();
        var result = ShortestStepPathIntersection(new Path(input[0]), new Path(input[1])).ToString();
        Console.WriteLine($"Time {(DateTimeOffset.Now - start).TotalMilliseconds}");
        return result;
    }

    private static int ClosestIntersection(Path path1, Path path2) {
        var points1 = path1.GetPathCoords();
        var points2 = path2.GetPathCoords();

        var path1Coords = new HashSet<(int x, int y)>();
        foreach (var p in points1) path1Coords.Add((p.x, p.y));

        // follow path 2 and see if it intersects with path 1
        var intersections = new List<(int x, int y)>();
        foreach (var p2 in points2) {
            if (path1Coords.Contains((p2.x, p2.y))) {
                intersections.Add((p2.x, p2.y));
            }
        }

        if (intersections.Count() == 0) {
            return 0;
        }

        var result = intersections.Where(i => i.x > 0 || i.y > 0).Min(ManhattanPath);
        return result;
    }

    private static int ShortestStepPathIntersection(Path path1, Path path2) {
        var points1 = path1.GetPathCoords();
        var points2 = path2.GetPathCoords();

        var p1Coords = new Dictionary<(int x, int y), List<(int x, int y, int steps)>>();
        foreach (var p in points1) {
            var coord = (p.x, p.y);
            if (!p1Coords.ContainsKey(coord)) {
                p1Coords[coord] = new List<(int x, int y, int steps)>();
            }
            p1Coords[coord].Add(p);
        }

        // follow path 2 and see if it intersects with path 1
        var intersections = new HashSet<(int x, int y, int steps)>();
        foreach (var p2 in points2) {
            var p2Coord = (p2.x, p2.y);
            
            if (p1Coords.ContainsKey(p2Coord)) {
                foreach (var intersection in p1Coords[p2Coord]) {
                    intersections.Add((intersection.x, intersection.y, intersection.steps + p2.steps));
                }
            }
        }

        if (intersections.Count() == 0) {
            return 0;
        }

        var result = intersections.Where(i => i.steps > 0).Min(i => i.steps);
        return result;
    }

    private static int ManhattanPath((int x, int y) point)
    {
        var result = Math.Abs(point.x) + Math.Abs(point.y);

        return result;
    }

    class Path {
        private readonly string[] steps;

        public Path(string[] steps)
        {
            this.steps = steps;
        }

        private static Regex stepRegex = new Regex(@"(\w)(\d+)");
        public (int x, int y)[] GetPoints() {
            var result = new List<(int x, int y)>();
            var current = (x: 0, y: 0);

            result.Add(current);

            foreach(var step in steps) {
                var match = stepRegex.Match(step);
                if (!match.Success) {
                    throw new ArgumentException();
                }
                var dir = match.Groups[1].Value;
                var length = int.Parse(match.Groups[2].Value);

                switch (dir) {
                    case "R": 
                        current = (x: current.x + length, y: current.y);
                        break;
                    case "L":
                        current = (x: current.x - length, y: current.y);
                        break;
                    case "U":
                        current = (x: current.x, y: current.y + length);
                        break;
                    case "D":
                        current = (x: current.x, y: current.y - length);
                        break;
                    default:
                        throw new ArgumentException();
                }

                result.Add(current);
            }

            return result.ToArray();
        }

        public List<(int x, int y, int steps)> GetPathCoords() {
            var result = new List<(int x, int y, int steps)>();
            var steps = 0;
            var nodes = GetPoints();
            for (int i = 1; i < nodes.Length; i++) {
                var p1 = nodes[i-1];
                var p2 = nodes[i];
                var current = p1;
                var destination = p2;
                result.Add((current.x, current.y, steps));
                while (current != destination) {
                    var newX = current.x + Math.Clamp(destination.x - current.x, -1, 1);
                    var newY = current.y + Math.Clamp(destination.y - current.y, -1, 1);

                    current = (newX, newY);
                    steps++;
                    result.Add((current.x, current.y, steps));
                }
            }

            return result;
        }
    }
}