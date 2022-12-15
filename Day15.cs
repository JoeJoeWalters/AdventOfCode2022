using System.Drawing;
using System.Text.Json;

namespace AdventOfCode2022
{
    public class Day15
    {
        /*
        --- Day 15: Beacon Exclusion Zone ---
        You feel the ground rumble again as the distress signal leads you to a large network of subterranean tunnels. You don't have time to search them all, but you don't need to: your pack contains a set of deployable sensors that you imagine were originally built to locate lost Elves.

        The sensors aren't very powerful, but that's okay; your handheld device indicates that you're close enough to the source of the distress signal to use them. You pull the emergency sensor system out of your pack, hit the big button on top, and the sensors zoom off down the tunnels.

        Once a sensor finds a spot it thinks will give it a good reading, it attaches itself to a hard surface and begins monitoring for the nearest signal source beacon. Sensors and beacons always exist at integer coordinates. Each sensor knows its own position and can determine the position of a beacon precisely; however, sensors can only lock on to the one beacon closest to the sensor as measured by the Manhattan distance. (There is never a tie where two beacons are the same distance to a sensor.)

        It doesn't take Int64 for the sensors to report back their positions and closest beacons (your puzzle input). For example:

        Sensor at x=2, y=18: closest beacon is at x=-2, y=15
        Sensor at x=9, y=16: closest beacon is at x=10, y=16
        Sensor at x=13, y=2: closest beacon is at x=15, y=3
        Sensor at x=12, y=14: closest beacon is at x=10, y=16
        Sensor at x=10, y=20: closest beacon is at x=10, y=16
        Sensor at x=14, y=17: closest beacon is at x=10, y=16
        Sensor at x=8, y=7: closest beacon is at x=2, y=10
        Sensor at x=2, y=0: closest beacon is at x=2, y=10
        Sensor at x=0, y=11: closest beacon is at x=2, y=10
        Sensor at x=20, y=14: closest beacon is at x=25, y=17
        Sensor at x=17, y=20: closest beacon is at x=21, y=22
        Sensor at x=16, y=7: closest beacon is at x=15, y=3
        Sensor at x=14, y=3: closest beacon is at x=15, y=3
        Sensor at x=20, y=1: closest beacon is at x=15, y=3
        So, consider the sensor at 2,18; the closest beacon to it is at -2,15. For the sensor at 9,16, the closest beacon to it is at 10,16.

        Drawing sensors as S and beacons as B, the above arrangement of sensors and beacons looks like this:

                       1    1    2    2
             0    5    0    5    0    5
         0 ....S.......................
         1 ......................S.....
         2 ...............S............
         3 ................SB..........
         4 ............................
         5 ............................
         6 ............................
         7 ..........S.......S.........
         8 ............................
         9 ............................
        10 ....B.......................
        11 ..S.........................
        12 ............................
        13 ............................
        14 ..............S.......S.....
        15 B...........................
        16 ...........SB...............
        17 ................S..........B
        18 ....S.......................
        19 ............................
        20 ............S......S........
        21 ............................
        22 .......................B....
        This isn't necessarily a comprehensive map of all beacons in the area, though. Because each sensor only identifies its closest beacon, if a sensor detects a beacon, you know there are no other beacons that close or closer to that sensor. There could still be beacons that just happen to not be the closest beacon to any sensor. Consider the sensor at 8,7:

                       1    1    2    2
             0    5    0    5    0    5
        -2 ..........#.................
        -1 .........###................
         0 ....S...#####...............
         1 .......#######........S.....
         2 ......#########S............
         3 .....###########SB..........
         4 ....#############...........
         5 ...###############..........
         6 ..#################.........
         7 .#########S#######S#........
         8 ..#################.........
         9 ...###############..........
        10 ....B############...........
        11 ..S..###########............
        12 ......#########.............
        13 .......#######..............
        14 ........#####.S.......S.....
        15 B........###................
        16 ..........#SB...............
        17 ................S..........B
        18 ....S.......................
        19 ............................
        20 ............S......S........
        21 ............................
        22 .......................B....
        This sensor's closest beacon is at 2,10, and so you know there are no beacons that close or closer (in any positions marked #).

        None of the detected beacons seem to be producing the distress signal, so you'll need to work out where the distress beacon is by working out where it isn't. For now, keep things simple by counting the positions where a beacon cannot possibly be aInt64 just a single row.

        So, suppose you have an arrangement of beacons and sensors like in the example above and, just in the row where y=10, you'd like to count the number of positions a beacon cannot possibly exist. The coverage from all sensors near that row looks like this:

                         1    1    2    2
               0    5    0    5    0    5
         9 ...#########################...
        10 ..####B######################..
        11 .###S#############.###########.
        In this example, in the row where y=10, there are 26 positions where a beacon cannot be present.

        Consult the report from the sensors you just deployed. In the row where y=2000000, how many positions cannot contain a beacon?
        */

        private const int maxCoord = 4000000;
        private Int64 Frequency(Int64 x, Int64 y) => (Int64)maxCoord * x + y;

        private Sensor[] CreateSensors(string[] input)
        {
            List<Sensor> sensors = new List<Sensor>();

            foreach (var line in input)
            {
                var lineData = line.Replace(", ", " ").Replace(": ", " ").Replace("=", " ").Split(" ");
                int sensorX = int.Parse(lineData[3]);
                int sensorY = int.Parse(lineData[5]);
                int beaconX = int.Parse(lineData[11]);
                int beaconY = int.Parse(lineData[13]);

                sensors.Add(new Sensor()
                {
                    X = sensorX,
                    Y = sensorY,
                    BeaconX = beaconX,
                    BeaconY = beaconY
                });
            }

            return sensors.ToArray();
        }

        private Int64 Part1(Sensor[] sensors, int yPos)
        {
            var positionsTo = new HashSet<int>();

            foreach (Sensor sensor in sensors)
            { 
                if (!sensor.Overlap(yPos))
                    continue;

                for (int x = (sensor.X - sensor.Range); x <= (sensor.X + sensor.Range); x++)
                {
                    if (sensor.CanReach(x, yPos))
                        positionsTo.Add(x);
                }

                if (sensor.BeaconY == yPos)
                    positionsTo.Remove(sensor.BeaconX);
            }

            return positionsTo.Count;
        }

        private Int64 Part2(Sensor[] sensors, int maxY)
        {
            // Filthy but context is king
            Boolean NotInRange(Sensor[] sensors, int x, int y)
            {
                for (int i = 0; i < sensors.Length; i++)
                {
                    if (sensors[i].CanReach(x, y))
                        return false;
                }

                return true;
            }

            foreach (Sensor sensor in sensors)
            {
                var edgeDistance = sensor.Range + 1;
                var min = Math.Max(sensor.X - edgeDistance, 0);
                var max = Math.Min(sensor.X + edgeDistance, maxCoord);

                for (int j = min; j < sensor.X; j++)
                {
                    int y1 = sensor.Y + (edgeDistance - (j - min));
                    if (y1 >= 0 && y1 <= maxY && NotInRange(sensors, j, y1))
                    {
                        return Frequency(j, y1);
                    }

                    int y2 = sensor.Y - (edgeDistance - (j - min));
                    if (y2 >= 0 && y2 <= maxY && NotInRange(sensors, j, y2))
                    {
                        return Frequency(j, y2);
                    }
                }

                var x = sensor.X;
                var y = sensor.Y + edgeDistance;
                if (y >= 0 && y <= maxY && NotInRange(sensors, x, y))
                {
                    return Frequency(x, y);
                }

                x = sensor.X;
                y = sensor.Y - edgeDistance;
                if (y >= 0 && y <= maxY && NotInRange(sensors, x, y))
                {
                    return Frequency(x, y);
                }

                for (int j = sensor.X + 1; j <= max; j++)
                {
                    int y1 = sensor.Y + (edgeDistance - (j - sensor.X));
                    if (y1 >= 0 && y1 <= maxY && NotInRange(sensors, j, y1))
                    {
                        return Frequency(j, y1);
                    }

                    int y2 = sensor.Y - (edgeDistance - (j - sensor.X));
                    if (y2 >= 0 && y2 <= maxY && NotInRange(sensors, j, y2))
                    {
                        return Frequency(j, y2);
                    }
                }
            }

            return 0;
        }
        
        private class Sensor
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int BeaconX { get; set; }
            public int BeaconY { get; set; }

            public int Range { get => Math.Abs(X - BeaconX) + Math.Abs(Y - BeaconY); }

            public Boolean CanReach(int x, int y) => (Math.Abs(X - x) + Math.Abs(Y - y) <= Range);
            public Boolean CanReach(Sensor other) => (Math.Abs(X - other.X) + Math.Abs(Y - other.Y) <= Range + other.Range + 1);
            public Boolean Overlap(int y) => (Math.Abs(Y - y) <= Range);
        }

        [Fact]
        public void Test()
        {
            // ARRANGE

            // Read all of the terminal input
            string[] lines = File.ReadAllLines(@".\Day15.txt");

            // ACT

            // ASSERT
            var sensors = CreateSensors(lines);
            var part1 = Part1(sensors, 2000000); // 6078701
            var part2 = Part2(sensors, maxCoord); // Int64.MaxValue); // 12567351400528
        }
    }

}
