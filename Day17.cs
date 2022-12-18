// Bit of a cheat to reduce the amount of code to write, the origional algorithm worked
// but the code was getting too messy to read, mental bandwidth was failing
using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2022
{
    public class Day17
    {
        /*
        --- Day 17: Pyroclastic Flow ---
        Your handheld device has located an alternative exit from the cave for you and the elephants. The ground is rumbling almost continuously now, but the strange valves bought you some time. It's definitely getting warmer in here, though.

        The tunnels eventually open into a very tall, narrow chamber. Large, oddly-shaped rocks are falling into the chamber from above, presumably due to all the rumbling. If you can't work out where the rocks will fall next, you might be crushed!

        The five types of rocks have the following peculiar shapes, where # is rock and . is empty space:

        ####

        .#.
        ###
        .#.

        ..#
        ..#
        ###

        #
        #
        #
        #

        ##
        ##
        The rocks fall in the order shown above: first the - shape, then the + shape, and so on. Once the end of the list is reached, the same order repeats: the - shape falls first, sixth, 11th, 16th, etc.

        The rocks don't spin, but they do get pushed around by jets of hot gas coming out of the walls themselves. A quick scan reveals the effect the jets of hot gas will have on the rocks as they fall (your puzzle input).

        For example, suppose this was the jet pattern in your cave:

        >>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>
        In jet patterns, < means a push to the left, while > means a push to the right. The pattern above means that the jets will push a falling rock right, then right, then right, then left, then left, then right, and so on. If the end of the list is reached, it repeats.

        The tall, vertical chamber is exactly seven units wide. Each rock appears so that its left edge is two units away from the left wall and its bottom edge is three units above the highest rock in the room (or the floor, if there isn't one).

        After a rock appears, it alternates between being pushed by a jet of hot gas one unit (in the direction indicated by the next symbol in the jet pattern) and then falling one unit down. If any movement would cause any part of the rock to move into the walls, floor, or a stopped rock, the movement instead does not occur. If a downward movement would have caused a falling rock to move into the floor or an already-fallen rock, the falling rock stops where it is (having landed on something) and a new rock immediately begins falling.

        Drawing falling rocks with @ and stopped rocks with #, the jet pattern in the example above manifests as follows:

        The first rock begins falling:
        |..@@@@.|
        |.......|
        |.......|
        |.......|
        +-------+

        Jet of gas pushes rock right:
        |...@@@@|
        |.......|
        |.......|
        |.......|
        +-------+

        Rock falls 1 unit:
        |...@@@@|
        |.......|
        |.......|
        +-------+

        Jet of gas pushes rock right, but nothing happens:
        |...@@@@|
        |.......|
        |.......|
        +-------+

        Rock falls 1 unit:
        |...@@@@|
        |.......|
        +-------+

        Jet of gas pushes rock right, but nothing happens:
        |...@@@@|
        |.......|
        +-------+

        Rock falls 1 unit:
        |...@@@@|
        +-------+

        Jet of gas pushes rock left:
        |..@@@@.|
        +-------+

        Rock falls 1 unit, causing it to come to rest:
        |..####.|
        +-------+

        A new rock begins falling:
        |...@...|
        |..@@@..|
        |...@...|
        |.......|
        |.......|
        |.......|
        |..####.|
        +-------+

        Jet of gas pushes rock left:
        |..@....|
        |.@@@...|
        |..@....|
        |.......|
        |.......|
        |.......|
        |..####.|
        +-------+

        Rock falls 1 unit:
        |..@....|
        |.@@@...|
        |..@....|
        |.......|
        |.......|
        |..####.|
        +-------+

        Jet of gas pushes rock right:
        |...@...|
        |..@@@..|
        |...@...|
        |.......|
        |.......|
        |..####.|
        +-------+

        Rock falls 1 unit:
        |...@...|
        |..@@@..|
        |...@...|
        |.......|
        |..####.|
        +-------+

        Jet of gas pushes rock left:
        |..@....|
        |.@@@...|
        |..@....|
        |.......|
        |..####.|
        +-------+

        Rock falls 1 unit:
        |..@....|
        |.@@@...|
        |..@....|
        |..####.|
        +-------+

        Jet of gas pushes rock right:
        |...@...|
        |..@@@..|
        |...@...|
        |..####.|
        +-------+

        Rock falls 1 unit, causing it to come to rest:
        |...#...|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        A new rock begins falling:
        |....@..|
        |....@..|
        |..@@@..|
        |.......|
        |.......|
        |.......|
        |...#...|
        |..###..|
        |...#...|
        |..####.|
        +-------+
        The moment each of the next few rocks begins falling, you would see this:

        |..@....|
        |..@....|
        |..@....|
        |..@....|
        |.......|
        |.......|
        |.......|
        |..#....|
        |..#....|
        |####...|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |..@@...|
        |..@@...|
        |.......|
        |.......|
        |.......|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |..@@@@.|
        |.......|
        |.......|
        |.......|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |...@...|
        |..@@@..|
        |...@...|
        |.......|
        |.......|
        |.......|
        |.####..|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |....@..|
        |....@..|
        |..@@@..|
        |.......|
        |.......|
        |.......|
        |..#....|
        |.###...|
        |..#....|
        |.####..|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |..@....|
        |..@....|
        |..@....|
        |..@....|
        |.......|
        |.......|
        |.......|
        |.....#.|
        |.....#.|
        |..####.|
        |.###...|
        |..#....|
        |.####..|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |..@@...|
        |..@@...|
        |.......|
        |.......|
        |.......|
        |....#..|
        |....#..|
        |....##.|
        |....##.|
        |..####.|
        |.###...|
        |..#....|
        |.####..|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+

        |..@@@@.|
        |.......|
        |.......|
        |.......|
        |....#..|
        |....#..|
        |....##.|
        |##..##.|
        |######.|
        |.###...|
        |..#....|
        |.####..|
        |....##.|
        |....##.|
        |....#..|
        |..#.#..|
        |..#.#..|
        |#####..|
        |..###..|
        |...#...|
        |..####.|
        +-------+
        To prove to the elephants your simulation is accurate, they want to know how tall the tower will get after 2022 rocks have stopped (but before the 2023rd rock begins falling). In this example, the tower of rocks will be 3068 units tall.

        How many units tall will the tower of rocks be after 2022 rocks have stopped falling?
        */

        private class Solver
        {
            private string[][] blocks = {
                new[] {
                    "@@@@"
                },
                new[] {
                    " @ ",
                    "@@@",
                    " @ "
                },
                new[] {
                    "@@@",
                    "  @",
                    "  @"
                },
                new[] {
                    "@",
                    "@",
                    "@",
                    "@"
                },
                new[] {
                    "@@",
                    "@@"
                }
            };

            private Dictionary<(int X, int Y), int> map = new Dictionary<(int X, int Y), int>();

            string data;

            private const long part1Cycles = 2023;
            private const long part2Depth = 1000000000000;

            public Solver(string data)
            {
                this.data = data;
            }

            // Calculate the result
            public long Result(int part)
            {
                // Answers to each part (not going to get caught out again doing one part different to second part and having to refactor)
                long part1 = 0;
                long part2 = 0;

                // Items to reproces as we do part 2
                Dictionary<(int Tape, int Shape), (long Rocks, long Height)> reprocess = new Dictionary<(int Tape, int Shape), (long Rocks, long Height)>();

                // The current block elements to check against
                string[] current;
                Point blockPos = new Point(0, 0); // The block position

                int index = 0; // The position in the data stream
                int maxY = 0; // Max Depth
                long cycles = 0; // How many cycles have we performed (more important for part 2), based on example, could be quite big

                // Finished flag
                bool finished = false;

                // Set the current block in the cycle
                int blockId = 0;
                current = blocks[block];

                while (part1 == 0 || part2 == 0)
                {
                    if (!finished)
                    {
                        blockPos.X = 2;
                        blockPos.Y = (map.Count > 1 ? maxY : -1) + 4;

                        current = blocks[blockId];
                        finished = true;

                        cycles++;

                        if (cycles == part1Cycles)
                            part1 = maxY + 1;
                    }

                    // Recalculate the block X axis if needed
                    int blockXCalc = blockPos.X;
                    blockXCalc += ((data[index] == '<') ? -1 : 1);

                    // Collision or out of bound on blockX?
                    if (0 <= blockXCalc && blockXCalc + current[0].Length <= 7 && CheckCollision(blockXCalc, blockPos.Y, current))
                        blockPos.X = blockXCalc; // reset blockX to be re-assigned the calculated value

                    // Same on the Y axis but more complex if hitting down on an "object"
                    int blockYCalc = blockPos.Y - 1;
                    index = (index + 1) % data.Length;

                    if (0 <= blockYCalc && CheckCollision(blockPos.X, blockYCalc, current))
                    {
                        blockPos.Y = blockYCalc;
                    }
                    else
                    {
                        // Cycle the block dimensions
                        for (int y = 0; y < blocks[blockId].Length; y++)
                        {
                            for (int x = 0; x < blocks[blockId][y].Length; x++)
                            {
                                // Part that can touch?
                                if (blocks[blockId][y][x] == '@')
                                    map[(blockPos.X + x, blockPos.Y + y)] = blockId;

                                // New maxY for part 2
                                maxY = Math.Max(maxY, blockPos.Y + y);
                            }
                        }

                        // Cycle to the next block on the list and if at the end modulo back around again
                        blockId = (blockId + 1) % blocks.Length;
                        finished = false;

                        if (reprocess.ContainsKey((index, blockId)) && part2 == 0)
                        {
                            var last = reprocess[(index, blockId)];
                            long cycle = cycles - last.Rocks;
                            long remainingCycles = part2Depth - cycles - 1;
                            long combinedCycles = (remainingCycles / (cycle) + 1);

                            if (cycles + combinedCycles * cycle == part2Depth)
                                part2 = maxY + 1 + combinedCycles * (maxY + 1 - last.Height);
                        }
                        else
                            reprocess[(index, blockId)] = (cycles, maxY + 1);
                    }
                }

                return (part == 1) ? part1 : part2; // Return the appropriate result to the caller
            }

            // Did the block collide with a fixed object?
            private bool CheckCollision(int blockX, int blockY, string[] current)
            {
                // Check the items in the cuurent block by cycling through it's dimensions
                for (int y = blockY; y < blockY + current.Length; y++)
                {
                    for (int x = blockX; x < blockX + current[y - blockY].Length; x++)
                    {
                        // Does the map correspond to an item we should worry about?
                        int found = -1;
                        if (map.ContainsKey((x, y)))
                            found = map[(x, y)];

                        // In the range of the current block?
                        if (blockY <= y && y <= blockY + current.Length - 1 &&
                            blockX <= x && x <= blockX + current[y - blockY].Length - 1)
                        {
                            // Matching an area that might indicate a collision? If so don't bother continuing, go right back
                            if (current[y - blockY][x - blockX] == '@' && found != -1)
                                return false;
                        }
                    }
                }

                return true;
            }
        }

        [Fact]
        public void Test()
        {
            // ARRANGE
            string data = File.ReadAllText(@".\Day17.txt");

            // ACT

            // ASSERT
            var part1 = (new Solver(data).Result(1)); // 3149
            var part2 = (new Solver(data).Result(2)); // 1553982300884
        }
    }

}