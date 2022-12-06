using System.Runtime.CompilerServices;

namespace AdventOfCode2022
{
    public class Day5
    {
        /*
        --- Day 5: Supply Stacks ---
        The expedition can depart as soon as the final supplies have been unloaded from the ships. Supplies are stored in stacks of marked crates, but because the needed supplies are buried under many other crates, the crates need to be rearranged.

        The ship has a giant cargo crane capable of moving crates between stacks. To ensure none of the crates get crushed or fall over, the crane operator will rearrange them in a series of carefully-planned steps. After the crates are rearranged, the desired crates will be at the top of each stack.

        The Elves don't want to interrupt the crane operator during this delicate procedure, but they forgot to ask her which crate will end up where, and they want to be ready to unload them as soon as possible so they can embark.

        They do, however, have a drawing of the starting stacks of crates and the rearrangement procedure (your puzzle input). For example:

            [D]    
        [N] [C]    
        [Z] [M] [P]
         1   2   3 

        move 1 from 2 to 1
        move 3 from 1 to 3
        move 2 from 2 to 1
        move 1 from 1 to 2
        In this example, there are three stacks of crates. Stack 1 contains two crates: crate Z is on the bottom, and crate N is on top. Stack 2 contains three crates; from bottom to top, they are crates M, C, and D. Finally, stack 3 contains a single crate, P.

        Then, the rearrangement procedure is given. In each step of the procedure, a quantity of crates is moved from one stack to a different stack. In the first step of the above rearrangement procedure, one crate is moved from stack 2 to stack 1, resulting in this configuration:

        [D]        
        [N] [C]    
        [Z] [M] [P]
         1   2   3 
        In the second step, three crates are moved from stack 1 to stack 3. Crates are moved one at a time, so the first crate to be moved (D) ends up below the second and third crates:

                [Z]
                [N]
            [C] [D]
            [M] [P]
         1   2   3
        Then, both crates are moved from stack 2 to stack 1. Again, because crates are moved one at a time, crate C ends up below crate M:

                [Z]
                [N]
        [M]     [D]
        [C]     [P]
         1   2   3
        Finally, one crate is moved from stack 1 to stack 2:

                [Z]
                [N]
                [D]
        [C] [M] [P]
         1   2   3
        The Elves just need to know which crate will end up on top of each stack; in this example, the top crates are C in stack 1, M in stack 2, and Z in stack 3, so you should combine these together and give the Elves the message CMZ.

        After the rearrangement procedure completes, what crate ends up on top of each stack?
        */

        [Fact]
        public void Test()
        {
            // ARRANGE
            string[] crateMoves = File.ReadAllLines(@".\Day5.txt");
            string[] setup = File.ReadAllLines(@".\Day5Setup.txt");

            Stack<char>[] stacks = new Stack<char>[9];

            // ACT

            // Setup the crates array for the starting point
            foreach (string stack in setup.Reverse()) // Stacks push down so start from the bottom up by reversing the data
            {
                var elements = stack.Chunk(4); // Each line can be split into the 9 stacks
                var i = 0;
                foreach (var element in elements)
                {
                    char crate = element[1];
                    if (stacks[i] == null)
                        stacks[i] = new Stack<char>();

                    if (crate != ' ')
                        stacks[i].Push(crate);

                    i++;
                }
            }

            // Now process the crates
            foreach (string move in crateMoves)
            {
                // How many to take from one stack and add to the other, remember they will be now upside down
                string[] moveParser = move.Split(' ');
                int take = int.Parse(moveParser[1]);
                int from = int.Parse(moveParser[3]) - 1;
                int to = int.Parse(moveParser[5]) - 1;

                //Part1Move(stacks, take, from, to);
                Part2Move(stacks, take, from, to);
            }

            // ASSERT
            string topStacks = String.Empty;
            foreach (Stack<char> stack in stacks)
            {
                topStacks = $"{topStacks}{stack.Peek()}";
            }
        }

        private void Part1Move(Stack<char>[] stacks, int take, int from, int to)
        {
            // How many to take from one stack and add to the other, remember they will be now upside down
            Stack<char> tempStack = new Stack<char>();
            for (var iterations = 0; iterations < take; iterations++)
            {
                stacks[to].Push(stacks[from].Pop()); // Pop from one stack and push to the other
            }
        }

        private void Part2Move(Stack<char>[] stacks, int take, int from, int to)
        {
            Stack<char> tempStack = new Stack<char>();
            for (var iterations = 0; iterations < take; iterations++)
            {
                tempStack.Push(stacks[from].Pop()); // Push to the temp stack which is now revered
            }

            // Push back to the next queue which puts back in the right order
            while (tempStack.Count != 0)
                stacks[to].Push(tempStack.Pop());
        }
    }
}