namespace AdventOfCode2022
{
    public class Day6
    {
        /*
        */

        [Fact]
        public void Test()
        {
            // ARRANGE
            char[] streamData = File.ReadAllText(@".\Day6.txt").ToCharArray();
            string receivedData = String.Empty; // Nothing is received at the start
            int markerLength = 14; // Part 1 = 4, Part 2 = 14

            // Stream to the elves once char at a time (we could cheat but lets not)
            for (int i = 0; i < streamData.Length; i++)
            {
                char c = streamData[i];
                receivedData = $"{receivedData}{c}";

                // Take the last X characters if they is enough
                if (receivedData.Length > markerLength)
                {
                    char[] chars = receivedData.Substring(receivedData.Length - markerLength, markerLength).ToCharArray();
                    bool distinct = chars.Take(markerLength).Distinct().Count() == markerLength;
                    if (distinct)
                    {
                        var answer = i + 1;
                    }
                }
            }

        }
    }
}