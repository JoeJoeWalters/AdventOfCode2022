using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    public class Day8
    {
        /*
        --- Day 8: Treetop Tree House ---
        The expedition comes across a peculiar patch of tall trees all planted carefully in a grid. The Elves explain that a previous expedition planted these trees as a reforestation effort. Now, they're curious if this would be a good location for a tree house.

        First, determine whether there is enough tree cover here to keep a tree house hidden. To do this, you need to count the number of trees that are visible from outside the grid when looking directly along a row or column.

        The Elves have already launched a quadcopter to generate a map with the height of each tree (your puzzle input). For example:

        30373
        25512
        65332
        33549
        35390
        Each tree is represented as a single digit whose value is its height, where 0 is the shortest and 9 is the tallest.

        A tree is visible if all of the other trees between it and an edge of the grid are shorter than it. Only consider trees in the same row or column; that is, only look up, down, left, or right from any given tree.

        All of the trees around the edge of the grid are visible - since they are already on the edge, there are no trees to block the view. In this example, that only leaves the interior nine trees to consider:

        The top-left 5 is visible from the left and top. (It isn't visible from the right or bottom since other trees of height 5 are in the way.)
        The top-middle 5 is visible from the top and right.
        The top-right 1 is not visible from any direction; for it to be visible, there would need to only be trees of height 0 between it and an edge.
        The left-middle 5 is visible, but only from the right.
        The center 3 is not visible from any direction; for it to be visible, there would need to be only trees of at most height 2 between it and an edge.
        The right-middle 3 is visible from the right.
        In the bottom row, the middle 5 is visible, but the 3 and 4 are not.
        With 16 trees visible on the edge and another 5 visible in the interior, a total of 21 trees are visible in this arrangement.

        Consider your map; how many trees are visible from outside the grid? 
        */

        private class Trees
        {
            private Point max;
            private Dictionary<Point, int> trees = new();

            public Trees(string[] data)
            {
                max = new Point(data[0].Length - 1, data.Count() - 1);
                for (int Y = 0; Y < data.Count(); Y++)
                {
                    var row = data[Y];
                    for (int X = 0; X < row.Length; X++)
                    {
                        trees[new(X, Y)] = int.Parse(row[X].ToString());
                    }
                }
            }

            public int Part1() => trees.Count(t => IsTreeVisible(t.Key));

            public int Part2() => trees.Max(t => GetVisibilityScore(t.Key));

            private int GetVisibilityScore(Point tree)
            {
                var height = trees[tree];
                int up = 0;
                int right = 0;
                int down = 0;
                int left = 0;

                // Check all directions

                for (int X = tree.X - 1; X >= 0; X--)
                {
                    left++;
                    if (trees[new(X, tree.Y)] >= height) 
                        break;
                }

                for (int X = tree.X + 1; X <= max.X; X++)
                {
                    right++;
                    if (trees[new(X, tree.Y)] >= height) 
                        break;
                }

                for (int Y = tree.Y - 1; Y >= 0; Y--)
                {
                    up++;
                    if (trees[new(tree.X, Y)] >= height) 
                        break;
                }

                for (int Y = tree.Y + 1; Y <= max.Y; Y++)
                {
                    down++;
                    if (trees[new(tree.X, Y)] >= height) 
                        break;
                }

                // Give the multiplier of the findings 
                return up * down * left * right;
            }

            // Tall enough to be visible?
            private bool IsTreeVisible(Point tree)
            {
                // Get all trees where the height is taller or equal to the given tree height
                var tallTrees = trees.Where(t => t.Value >= trees[tree]).ToList();

                // Check in all directions
                var up = !tallTrees.Where(t => t.Key.X == tree.X && t.Key.Y < tree.Y).Any();
                var down = !tallTrees.Where(t => t.Key.X == tree.X && t.Key.Y > tree.Y).Any();
                var left = !tallTrees.Where(t => t.Key.X < tree.X && t.Key.Y == tree.Y).Any();
                var right = !tallTrees.Where(t => t.Key.X > tree.X && t.Key.Y == tree.Y).Any();

                // Any? Yes? Then cast to boolean if any are true
                return up || down || left || right;
            }
        }


        [Fact]
        public void Test()
        {
            // ARRANGE

            // Read all of the terminal input
            string[] data = File.ReadAllLines(@".\Day8.txt");
            var trees = new Trees(data);

            // ACT

            // ASSERT
            var part1Result = trees.Part1();
            var part2Result = trees.Part2();
        }
    }

}
