// Bit of a cheat to reduce the amount of code to write, the origional algorithm worked
// but the code was getting too messy to read, mental bandwidth was failing
using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

using System.Text.RegularExpressions;

namespace AdventOfCode2022
{
    public class Day16
    {
        /*
        --- Day 16: Proboscidea Volcanium ---
        The sensors have led you to the origin of the distress signal: yet another handheld device, just like the one the Elves gave you. However, you don't see any Elves around; instead, the device is surrounded by elephants! They must have gotten lost in these tunnels, and one of the elephants apparently figured out how to turn on the distress signal.

        The ground rumbles again, much stronger this time. What kind of cave is this, exactly? You scan the cave with your handheld device; it reports mostly igneous rock, some ash, pockets of pressurized gas, magma... this isn't just a cave, it's a volcano!

        You need to get the elephants out of here, quickly. Your device estimates that you have 30 minutes before the volcano erupts, so you don't have time to go back out the way you came in.

        You scan the cave for other options and discover a network of pipes and pressure-release valves. You aren't sure how such a system got into a volcano, but you don't have time to complain; your device produces a report (your puzzle input) of each valve's flow rate if it were opened (in pressure per minute) and the tunnels you could use to move between the valves.

        There's even a valve in the room you and the elephants are currently standing in labeled AA. You estimate it will take you one minute to open a single valve and one minute to follow any tunnel from one valve to another. What is the most pressure you could release?

        For example, suppose you had the following scan output:

        Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
        Valve BB has flow rate=13; tunnels lead to valves CC, AA
        Valve CC has flow rate=2; tunnels lead to valves DD, BB
        Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
        Valve EE has flow rate=3; tunnels lead to valves FF, DD
        Valve FF has flow rate=0; tunnels lead to valves EE, GG
        Valve GG has flow rate=0; tunnels lead to valves FF, HH
        Valve HH has flow rate=22; tunnel leads to valve GG
        Valve II has flow rate=0; tunnels lead to valves AA, JJ
        Valve JJ has flow rate=21; tunnel leads to valve II
        All of the valves begin closed. You start at valve AA, but it must be damaged or jammed or something: its flow rate is 0, so there's no point in opening it. However, you could spend one minute moving to valve BB and another minute opening it; doing so would release pressure during the remaining 28 minutes at a flow rate of 13, a total eventual pressure release of 28 * 13 = 364. Then, you could spend your third minute moving to valve CC and your fourth minute opening it, providing an additional 26 minutes of eventual pressure release at a flow rate of 2, or 52 total pressure released by valve CC.

        Making your way through the tunnels like this, you could probably open many or all of the valves by the time 30 minutes have elapsed. However, you need to release as much pressure as possible, so you'll need to be methodical. Instead, consider this approach:

        == Minute 1 ==
        No valves are open.
        You move to valve DD.

        == Minute 2 ==
        No valves are open.
        You open valve DD.

        == Minute 3 ==
        Valve DD is open, releasing 20 pressure.
        You move to valve CC.

        == Minute 4 ==
        Valve DD is open, releasing 20 pressure.
        You move to valve BB.

        == Minute 5 ==
        Valve DD is open, releasing 20 pressure.
        You open valve BB.

        == Minute 6 ==
        Valves BB and DD are open, releasing 33 pressure.
        You move to valve AA.

        == Minute 7 ==
        Valves BB and DD are open, releasing 33 pressure.
        You move to valve II.

        == Minute 8 ==
        Valves BB and DD are open, releasing 33 pressure.
        You move to valve JJ.

        == Minute 9 ==
        Valves BB and DD are open, releasing 33 pressure.
        You open valve JJ.

        == Minute 10 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve II.

        == Minute 11 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve AA.

        == Minute 12 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve DD.

        == Minute 13 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve EE.

        == Minute 14 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve FF.

        == Minute 15 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve GG.

        == Minute 16 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You move to valve HH.

        == Minute 17 ==
        Valves BB, DD, and JJ are open, releasing 54 pressure.
        You open valve HH.

        == Minute 18 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You move to valve GG.

        == Minute 19 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You move to valve FF.

        == Minute 20 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You move to valve EE.

        == Minute 21 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You open valve EE.

        == Minute 22 ==
        Valves BB, DD, EE, HH, and JJ are open, releasing 79 pressure.
        You move to valve DD.

        == Minute 23 ==
        Valves BB, DD, EE, HH, and JJ are open, releasing 79 pressure.
        You move to valve CC.

        == Minute 24 ==
        Valves BB, DD, EE, HH, and JJ are open, releasing 79 pressure.
        You open valve CC.

        == Minute 25 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        == Minute 26 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        == Minute 27 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        == Minute 28 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        == Minute 29 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        == Minute 30 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.
        This approach lets you release the most pressure possible in 30 minutes with this valve layout, 1651.

        Work out the steps to release the most pressure in 30 minutes. What is the most pressure you can release?

        Your puzzle answer was 1796.

        The first half of this puzzle is complete! It provides one gold star: *

        --- Part Two ---
        You're worried that even with an optimal approach, the pressure released won't be enough. What if you got one of the elephants to help you?

        It would take you 4 minutes to teach an elephant how to open the right valves in the right order, leaving you with only 26 minutes to actually execute your plan. Would having two of you working together be better, even if it means having less time? (Assume that you teach the elephant before opening any valves yourself, giving you both the same full 26 minutes.)

        In the example above, you could teach the elephant to help you as follows:

        == Minute 1 ==
        No valves are open.
        You move to valve II.
        The elephant moves to valve DD.

        == Minute 2 ==
        No valves are open.
        You move to valve JJ.
        The elephant opens valve DD.

        == Minute 3 ==
        Valve DD is open, releasing 20 pressure.
        You open valve JJ.
        The elephant moves to valve EE.

        == Minute 4 ==
        Valves DD and JJ are open, releasing 41 pressure.
        You move to valve II.
        The elephant moves to valve FF.

        == Minute 5 ==
        Valves DD and JJ are open, releasing 41 pressure.
        You move to valve AA.
        The elephant moves to valve GG.

        == Minute 6 ==
        Valves DD and JJ are open, releasing 41 pressure.
        You move to valve BB.
        The elephant moves to valve HH.

        == Minute 7 ==
        Valves DD and JJ are open, releasing 41 pressure.
        You open valve BB.
        The elephant opens valve HH.

        == Minute 8 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You move to valve CC.
        The elephant moves to valve GG.

        == Minute 9 ==
        Valves BB, DD, HH, and JJ are open, releasing 76 pressure.
        You open valve CC.
        The elephant moves to valve FF.

        == Minute 10 ==
        Valves BB, CC, DD, HH, and JJ are open, releasing 78 pressure.
        The elephant moves to valve EE.

        == Minute 11 ==
        Valves BB, CC, DD, HH, and JJ are open, releasing 78 pressure.
        The elephant opens valve EE.

        (At this point, all valves are open.)

        == Minute 12 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        ...

        == Minute 20 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.

        ...

        == Minute 26 ==
        Valves BB, CC, DD, EE, HH, and JJ are open, releasing 81 pressure.
        With the elephant helping, after 26 minutes, the best you could do would release a total of 1707 pressure.

        With you and an elephant working together for 26 minutes, what is the most pressure you could release?
        */

        private class Valve
        {
            public uint Node { get; set; } // Graph Id 
            public string Name { get; set; } = string.Empty; // Input reference
            public int Flow { get; set; } // The Flow rate
            public List<uint> Tunnels { get; set; } = new List<uint>(); // list of tunnel Id's from Graph
            public List<string> TunnelRefs { get; set; } = new List<string>(); // list of tunnel Id's from input which we will transform to id's from graph
        }

        private class TimeScoreMarker
        {
            public int Time { get; set; }
            public int Score { get; set; }
        }

        private class Solver
        {
            private int score = 0;
            private Graph graph = new Graph(); // Let the Dijkstra library do the donkey work

            private const string firstValve = "AA";

            // Unsigned ints because that is what Dijkstra uses and I can't be bothered :) 
            Dictionary<uint, Valve> valves;
            Dictionary<(uint, uint), int> pathLengths;

            uint[] activeFlows;
            uint startingValve;

            public Solver(string[] data)
            {
                // Parse the data (but we need to link the data to the Graph nodes after once it has been parsed)
                var preParse = data
                    .Select(line =>
                            line.Replace("=", " ").Replace(";", "").Replace(", ", ",").Replace(":", "").Split(" ") // Change the line then split it up to know where the parts will be
                        )
                    .Select(parts =>
                            new Valve()
                            {
                                Node = graph.AddNode(),
                                Name = parts[1],
                                Flow = int.Parse(parts[5]),
                                TunnelRefs = parts[parts.Length - 1].Split(",").ToList()
                            }
                            )
                    .ToDictionary(valve => valve.Name, valve => valve);

                // Bind the tunnel references to a graph item reference
                foreach (var valve in preParse)
                {
                    // Update the tunnel list based on the nodes created in the graph
                    valve.Value.Tunnels = valve.Value.TunnelRefs.Select(s => preParse[s].Node).ToList(); // Bind the tunnel references to graph references

                    // Get the starting valve based on it's reference
                    if (valve.Value.Name == firstValve)
                        startingValve = valve.Value.Node;
                }

                // Create a new dictionary where the key is now the graph reference not the text key
                valves = preParse.Values.Select(valve => valve).ToDictionary(valve => valve.Node, valve => valve);

                // Connect up all the valves in the graph so we can do the calculation
                foreach (var valve in valves.Values)
                {
                    foreach (var to in valve.Tunnels)
                        graph.Connect(valve.Node, to, 1);
                }

                // Build a new dictionary to hold the length of the paths that have been found between points and then store
                pathLengths = new Dictionary<(uint, uint), int>();

                // Which valves have a possible flow value to quicken the lookup
                activeFlows = valves.Values.Where(valve => valve.Flow > 0).Select(valve => valve.Node).ToArray();
            }

            // Generate the result for the part 
            public int Result(int part)
            {
                score = 0; // Reset the score then start the search
                int time = (part == 1 ? 30 : 26);
                Search(startingValve, activeFlows, 
                        new TimeScoreMarker() 
                        { 
                            Time = time, 
                            Score = 0 
                        }, (part == 1));
                return score;
            }

            // What can the next shortest move be? Divest to the graph routine
            private TimeScoreMarker CalculateNextMove(uint from, uint to, TimeScoreMarker previous)
            {
                if (!pathLengths.TryGetValue((from, to), out var dist))
                {
                    dist = graph.Dijkstra(from, to).Distance;
                    pathLengths.Add((from, to), dist);
                }

                var timeAtEnd = previous.Time - dist - 1;
                return new TimeScoreMarker()
                {
                    Time = timeAtEnd,
                    Score = previous.Score + (timeAtEnd * valves[to].Flow)
                };
            }

            // Search for valves to switch
            private void Search(uint from, uint[] flows, TimeScoreMarker previous, bool isElephantMove)
            {
                for (var i = 0; i < flows.Length; i++)
                {
                    var to = flows[i];
                    var nextMove = CalculateNextMove(from, to, previous);
                    if (nextMove.Time >= 0)
                    {
                        if (nextMove.Score > score)
                            score = nextMove.Score;

                        if (flows.Length > 1)
                            Search(to, flows.Where(j => j != to).ToArray(), nextMove, isElephantMove); // Exclude the last flow checked and move on to the next sub-search
                    }
                    else if (!isElephantMove && previous.Score >= score / 2)
                    {
                        Search(startingValve, flows,
                            new TimeScoreMarker()
                            {
                                Time = 26,
                                Score = previous.Score
                            }, true);
                    }
                }
            }
        }

        [Fact]
        public void Test()
        {
            // ARRANGE
            string[] lines = File.ReadAllLines(@".\Day16.txt");

            // ACT

            // ASSERT
            var part1 = new Solver(lines).Result(1); // 1796
            var part2 = new Solver(lines).Result(2); // 1999
        }
    }

}
