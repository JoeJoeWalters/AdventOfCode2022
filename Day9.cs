﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    public class Day9
    {
        /*
        --- Day 9: Rope Bridge ---
        This rope bridge creaks as you walk along it. You aren't sure how old it is, or whether it can even support your weight.

        It seems to support the Elves just fine, though. The bridge spans a gorge which was carved out by the massive river far below you.

        You step carefully; as you do, the ropes stretch and twist. You decide to distract yourself by modeling rope physics; maybe you can even figure out where not to step.

        Consider a rope with a knot at each end; these knots mark the head and the tail of the rope. If the head moves far enough away from the tail, the tail is pulled toward the head.

        Due to nebulous reasoning involving Planck lengths, you should be able to model the positions of the knots on a two-dimensional grid. Then, by following a hypothetical series of motions (your puzzle input) for the head, you can determine how the tail will move.

        Due to the aforementioned Planck lengths, the rope must be quite short; in fact, the head (H) and tail (T) must always be touching (diagonally adjacent and even overlapping both count as touching):

        ....
        .TH.
        ....

        ....
        .H..
        ..T.
        ....

        ...
        .H. (H covers T)
        ...
        If the head is ever two steps directly up, down, left, or right from the tail, the tail must also move one step in that direction so it remains close enough:

        .....    .....    .....
        .TH.. -> .T.H. -> ..TH.
        .....    .....    .....

        ...    ...    ...
        .T.    .T.    ...
        .H. -> ... -> .T.
        ...    .H.    .H.
        ...    ...    ...
        Otherwise, if the head and tail aren't touching and aren't in the same row or column, the tail always moves one step diagonally to keep up:

        .....    .....    .....
        .....    ..H..    ..H..
        ..H.. -> ..... -> ..T..
        .T...    .T...    .....
        .....    .....    .....

        .....    .....    .....
        .....    .....    .....
        ..H.. -> ...H. -> ..TH.
        .T...    .T...    .....
        .....    .....    .....
        You just need to work out where the tail goes as the head follows a series of motions. Assume the head and the tail both start at the same position, overlapping.

        For example:

        R 4
        U 4
        L 3
        D 1
        R 4
        D 1
        L 5
        R 2
        This series of motions moves the head right four steps, then up four steps, then left three steps, then down one step, and so on. After each step, you'll need to update the position of the tail if the step means the head is no longer adjacent to the tail. Visually, these motions occur as follows (s marks the starting position as a reference point):

        == Initial State ==

        ......
        ......
        ......
        ......
        H.....  (H covers T, s)

        == R 4 ==

        ......
        ......
        ......
        ......
        TH....  (T covers s)

        ......
        ......
        ......
        ......
        sTH...

        ......
        ......
        ......
        ......
        s.TH..

        ......
        ......
        ......
        ......
        s..TH.

        == U 4 ==

        ......
        ......
        ......
        ....H.
        s..T..

        ......
        ......
        ....H.
        ....T.
        s.....

        ......
        ....H.
        ....T.
        ......
        s.....

        ....H.
        ....T.
        ......
        ......
        s.....

        == L 3 ==

        ...H..
        ....T.
        ......
        ......
        s.....

        ..HT..
        ......
        ......
        ......
        s.....

        .HT...
        ......
        ......
        ......
        s.....

        == D 1 ==

        ..T...
        .H....
        ......
        ......
        s.....

        == R 4 ==

        ..T...
        ..H...
        ......
        ......
        s.....

        ..T...
        ...H..
        ......
        ......
        s.....

        ......
        ...TH.
        ......
        ......
        s.....

        ......
        ....TH
        ......
        ......
        s.....

        == D 1 ==

        ......
        ....T.
        .....H
        ......
        s.....

        == L 5 ==

        ......
        ....T.
        ....H.
        ......
        s.....

        ......
        ....T.
        ...H..
        ......
        s.....

        ......
        ......
        ..HT..
        ......
        s.....

        ......
        ......
        .HT...
        ......
        s.....

        ......
        ......
        HT....
        ......
        s.....

        == R 2 ==

        ......
        ......
        .H....  (H covers T)
        ......
        s.....

        ......
        ......
        .TH...
        ......
        s.....
        After simulating the rope, you can count up all of the positions the tail visited at least once. In this diagram, s again marks the starting position (which the tail also visited) and # marks other positions the tail visited:

        ..##..
        ...##.
        .####.
        ....#.
        s###..
        So, there are 13 positions the tail visited at least once.

        Simulate your complete hypothetical series of motions. How many positions does the tail of the rope visit at least once?
        */

        // Transalte the line value depending on which direction was travelled, e.g. U = Up so +1 etc.
        private int CastLineValue(char value, bool y)
            => y ? value switch { 'U' => 1, 'D' => -1, _ => 0 }
                : value switch { 'R' => 1, 'L' => -1, _ => 0 };

        // Oh lord I used a tuple .. Kill me now
        private (int, int) ApplyDiff((int x, int y) rope, (int x, int y) diff)
            => (ApplyAxisDiff(rope.x, diff.x), ApplyAxisDiff(rope.y, diff.y));

        // Essentially just a Math.Sign (direction of travel) but wanted to expand it 
        // so it was understandable what I was doing
        private int ApplyAxisDiff(int position, int directionDistance)
        {
            int moveAmount = 0;
            switch (directionDistance) 
            {
                case < 0:
                    moveAmount = -1;
                    break;
                case > 0:
                    moveAmount = 1;
                    break;
            }

            return moveAmount + position; // Push the position in the right direction for this axis
        }

        private int HowMany(string[] data, int knots)
        {
            // Create a rope with a the right number of knots
            HashSet<(int x, int y)> positions = new();
            List<(int x, int y)> rope = new();
            for (int i = 0; i < knots; i++)
            {
                rope.Add((0, 0));
            }

            // Loop through the data provided
            foreach (string line in data)
            {
                // Get the amount of the correct position in the line then get the value by parsing the character given
                int amount = int.Parse(line[2..]);
                int xd = CastLineValue(line[0], false);
                int yd = CastLineValue(line[0], true);

                // Loop back down the aount as long as we are over 0
                while (amount > 0)
                {
                    // First item is the offset
                    rope[0] = (rope[0].x + xd, rope[0].y + yd);

                    // Loop through the rope knots
                    for (int i = 1; i < rope.Count; i++)
                    {
                        // Difference between positions
                        (int x, int y) diff = (rope[i - 1].x - rope[i].x, rope[i - 1].y - rope[i].y);
                        if (Math.Abs(diff.x) > 1 || Math.Abs(diff.y) > 1)
                        {
                            // Force in the right direction
                            rope[i] = ApplyDiff(rope[i], diff);
                        }
                    }

                    // Add the last item as to the list of positions
                    positions.Add(rope[rope.Count() - 1]);

                    // Move to the previous
                    amount--;
                }
            }

            return positions.Count;
        }

        [Fact]
        public void Test()
        {
            // ARRANGE

            // Read all of the terminal input
            string[] data = File.ReadAllLines(@".\Day9.txt");

            // ACT

            // ASSERT
            var part1 = HowMany(data, 2);
            var part2 = HowMany(data, 10);

        }
    }

}