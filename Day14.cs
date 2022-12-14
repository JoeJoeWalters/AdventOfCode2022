﻿using System.Drawing;
using System.Text.Json;

namespace AdventOfCode2022
{
    public class Day14
    {
        /*
        --- Day 14: Regolith Reservoir ---
        The distress signal leads you to a giant waterfall! Actually, hang on - the signal seems like it's coming from the waterfall itself, and that doesn't make any sense. However, you do notice a little path that leads behind the waterfall.

        Correction: the distress signal leads you behind a giant waterfall! There seems to be a large cave system here, and the signal definitely leads further inside.

        As you begin to make your way deeper underground, you feel the ground rumble for a moment. Sand begins pouring into the cave! If you don't quickly figure out where the sand is going, you could quickly become trapped!

        Fortunately, your familiarity with analyzing the path of falling material will come in handy here. You scan a two-dimensional vertical slice of the cave above you (your puzzle input) and discover that it is mostly air with structures made of rock.

        Your scan traces the path of each solid rock structure and reports the x,y coordinates that form the shape of the path, where x represents distance to the right and y represents distance down. Each path appears as a single line of text in your scan. After the first point of each path, each point indicates the end of a straight horizontal or vertical line to be drawn from the previous point. For example:

        498,4 -> 498,6 -> 496,6
        503,4 -> 502,4 -> 502,9 -> 494,9
        This scan means that there are two paths of rock; the first path consists of two straight lines, and the second path consists of three straight lines. (Specifically, the first path consists of a line of rock from 498,4 through 498,6 and another line of rock from 498,6 through 496,6.)

        The sand is pouring into the cave from point 500,0.

        Drawing rock as #, air as ., and the source of the sand as +, this becomes:


            4     5  5
            9     0  0
            4     0  3
        0 ......+...
        1 ..........
        2 ..........
        3 ..........
        4 ....#...##
        5 ....#...#.
        6 ..###...#.
        7 ........#.
        8 ........#.
        9 #########.
        Sand is produced one unit at a time, and the next unit of sand is not produced until the previous unit of sand comes to rest. A unit of sand is large enough to fill one tile of air in your scan.

        A unit of sand always falls down one step if possible. If the tile immediately below is blocked (by rock or sand), the unit of sand attempts to instead move diagonally one step down and to the left. If that tile is blocked, the unit of sand attempts to instead move diagonally one step down and to the right. Sand keeps moving as long as it is able to do so, at each step trying to move down, then down-left, then down-right. If all three possible destinations are blocked, the unit of sand comes to rest and no longer moves, at which point the next unit of sand is created back at the source.

        So, drawing sand that has come to rest as o, the first unit of sand simply falls straight down and then stops:

        ......+...
        ..........
        ..........
        ..........
        ....#...##
        ....#...#.
        ..###...#.
        ........#.
        ......o.#.
        #########.
        The second unit of sand then falls straight down, lands on the first one, and then comes to rest to its left:

        ......+...
        ..........
        ..........
        ..........
        ....#...##
        ....#...#.
        ..###...#.
        ........#.
        .....oo.#.
        #########.
        After a total of five units of sand have come to rest, they form this pattern:

        ......+...
        ..........
        ..........
        ..........
        ....#...##
        ....#...#.
        ..###...#.
        ......o.#.
        ....oooo#.
        #########.
        After a total of 22 units of sand:

        ......+...
        ..........
        ......o...
        .....ooo..
        ....#ooo##
        ....#ooo#.
        ..###ooo#.
        ....oooo#.
        ...ooooo#.
        #########.
        Finally, only two more units of sand can possibly come to rest:

        ......+...
        ..........
        ......o...
        .....ooo..
        ....#ooo##
        ...o#ooo#.
        ..###ooo#.
        ....oooo#.
        .o.ooooo#.
        #########.
        Once all 24 units of sand shown above have come to rest, all further sand flows out the bottom, falling into the endless void. Just for fun, the path any new sand takes before falling forever is shown here with ~:

        .......+...
        .......~...
        ......~o...
        .....~ooo..
        ....~#ooo##
        ...~o#ooo#.
        ..~###ooo#.
        ..~..oooo#.
        .~o.ooooo#.
        ~#########.
        ~..........
        ~..........
        ~..........
        Using your scan, simulate the falling sand. How many units of sand come to rest before sand starts flowing into the abyss below?
        */

        internal const int maxDimension = 500;

        internal class Grid
        {
            public HashSet<Point> points { get; }
            public int minX { get; }
            public int maxX { get; }
            public int maxY { get; }

            private Point startPoint = new Point(maxDimension, 0);

            public Grid(HashSet<Point> points, int minX, int maxX, int maxY)
            {
                this.points = points;
                this.minX = minX;
                this.maxX = maxX;
                this.maxY = maxY;
            }

            public int Count(bool hasFloor)
            {
                var count = 0;
                while (Add(startPoint, hasFloor))
                    count++;
                return hasFloor ? count + 1 : count;
            }

            private bool Add(Point point, bool hasFloor)
            {
                var nextPoints = new[] {
                        new Point(point.X, point.Y + 1),
                        new Point(point.X - 1, point.Y + 1),
                        new Point(point.X + 1, point.Y + 1)
                      };

                foreach (var next in nextPoints)
                {
                    if (!hasFloor && (next.X < minX || next.X > maxX || next.Y > maxY))
                        return false;

                    if (!points.Contains(next) && (!hasFloor || next.Y < maxY + 2))
                        return Add(next, hasFloor);
                }

                if (point.Equals(startPoint))
                    return false;

                points.Add(point);
                return true;
            }

            public void Print()
            {
                for (int y = 0; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        var point = new Point(x, y);
                        char cell = points.Contains(point) ? '#' : '.';
                        Console.Write(cell);
                    }
                    Console.WriteLine();
                }
            }
        }

        internal Point Parse(string pointString)
        {
            string[] xy = pointString.Split(',');
            var point = new Point(Int32.Parse(xy[0]), Int32.Parse(xy[1]));
            return point;
        }

        internal Grid CreateGrid(string[] lines)
        {
            var points = new HashSet<Point>();

            // Remember the sand is a pyramid to cannot be wider than it is high in this situation
            var minX = maxDimension;
            var maxX = maxDimension;
            var maxY = 0;

            foreach (var line in lines)
            {
                string[] instructions = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < instructions.Length - 1; i++)
                {
                    var start = Parse(instructions[i]);
                    var end = Parse(instructions[i + 1]);

                    var startX = (start.X <= end.X) ? start.X : end.X;
                    var endX = (start.X <= end.X) ? end.X : start.X;
                    
                    minX = Math.Min(startX, minX);
                    maxX = Math.Max(endX, maxX);

                    for (int x = startX; x <= endX; x++)
                    {
                        var startY = Math.Min(start.Y, end.Y);
                        var endY = Math.Max(start.Y, end.Y);
                        maxY = Math.Max(endY, maxY);

                        for (int y = startY; y <= endY; y++)
                        {
                            var point = new Point(x, y);
                            points.Add(point);
                        }
                    }
                }
            }

            return new Grid(points, minX, maxX, maxY);
        }


        [Fact]
        public void Test()
        {
            // ARRANGE

            // Read all of the terminal input
            string[] lines = File.ReadAllLines(@".\Day14.txt");

            // ACT

            // ASSERT
            var part1 = CreateGrid(lines).Count(false);
            var part2 = CreateGrid(lines).Count(true);
        }
    }

}