namespace AdventOfCode2022
{
    public class Day7
    {
        /*
         * You can hear birds chirping and raindrops hitting leaves as the expedition proceeds. Occasionally, you can even hear much louder sounds in the distance; how big do the animals get out here, anyway?

        The device the Elves gave you has problems with more than just its communication system. You try to run a system update:

        $ system-update --please --pretty-please-with-sugar-on-top
        Error: No space left on device
        Perhaps you can delete some files to make space for the update?

        You browse around the filesystem to assess the situation and save the resulting terminal output (your puzzle input). For example:

        $ cd /
        $ ls
        dir a
        14848514 b.txt
        8504156 c.dat
        dir d
        $ cd a
        $ ls
        dir e
        29116 f
        2557 g
        62596 h.lst
        $ cd e
        $ ls
        584 i
        $ cd ..
        $ cd ..
        $ cd d
        $ ls
        4060174 j
        8033020 d.log
        5626152 d.ext
        7214296 k
        The filesystem consists of a tree of files (plain data) and directories (which can contain other directories or files). The outermost directory is called /. You can navigate around the filesystem, moving into or out of directories and listing the contents of the directory you're currently in.

        Within the terminal output, lines that begin with $ are commands you executed, very much like some modern computers:

        cd means change directory. This changes which directory is the current directory, but the specific result depends on the argument:
        cd x moves in one level: it looks in the current directory for the directory named x and makes it the current directory.
        cd .. moves out one level: it finds the directory that contains the current directory, then makes that directory the current directory.
        cd / switches the current directory to the outermost directory, /.
        ls means list. It prints out all of the files and directories immediately contained by the current directory:
        123 abc means that the current directory contains a file named abc with size 123.
        dir xyz means that the current directory contains a directory named xyz.
        Given the commands and output in the example above, you can determine that the filesystem looks visually like this:

        - / (dir)
          - a (dir)
            - e (dir)
              - i (file, size=584)
            - f (file, size=29116)
            - g (file, size=2557)
            - h.lst (file, size=62596)
          - b.txt (file, size=14848514)
          - c.dat (file, size=8504156)
          - d (dir)
            - j (file, size=4060174)
            - d.log (file, size=8033020)
            - d.ext (file, size=5626152)
            - k (file, size=7214296)
        Here, there are four directories: / (the outermost directory), a and d (which are in /), and e (which is in a). These directories also contain files of various sizes.

        Since the disk is full, your first step should probably be to find directories that are good candidates for deletion. To do this, you need to determine the total size of each directory. The total size of a directory is the sum of the sizes of the files it contains, directly or indirectly. (Directories themselves do not count as having any intrinsic size.)

        The total sizes of the directories above can be found as follows:

        The total size of directory e is 584 because it contains a single file i of size 584 and no other directories.
        The directory a has total size 94853 because it contains files f (size 29116), g (size 2557), and h.lst (size 62596), plus file i indirectly (a contains e which contains i).
        Directory d has total size 24933642.
        As the outermost directory, / contains every file. Its total size is 48381165, the sum of the size of every file.
        To begin, find all of the directories with a total size of at most 100000, then calculate the sum of their total sizes. In the example above, these directories are a and e; the sum of their total sizes is 95437 (94853 + 584). (As in this example, this process can count files more than once!)

        Find all of the directories with a total size of at most 100000. What is the sum of the total sizes of those directories?
        */

        [Fact]
        public void Test()
        {
            // ARRANGE
            int totalSpaceAvailable = 70000000;
            int neededSpace = 30000000;

            // Read all of the terminal input
            string[] lines = File.ReadAllLines(@".\Day7.txt");

            // ACT

            // Generate a directory scheme by hijacking the Microsoft system object
            var path = "";
            var paths = new Dictionary<string, int>(); // Name, size

            // Manually loop through the lines rather than doing a for each as we may need to manually skip some lines
            // if we find a directory listing
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                // Only deal with command lines (we will manually move through any output in lines listed afterwards so we want to ignore everything else)
                if (line.StartsWith('$'))
                {
                    // Break up the command into chunks
                    var parts = line.Split(' ');
                    var command = parts[1]; // Command must be the second element
                    var args = parts.Length > 2 ? parts.Last() : ""; // Arguments have a specific place depending on the type

                    // Now we have the command, take the input and operate it
                    switch (command)
                    {
                        case "cd":

                            // Path is revealed to us from the args so get it stored for later as we will always be in this directory now
                            path = Path.GetFullPath(Path.Combine(path, args));

                            break;

                        case "ls":

                            // Grab all lines after the directory listing that are no commands until we hit one
                            var output = lines.Skip(i + 1).TakeWhile(f => !f.StartsWith('$'));                             
                            
                            // Get the file sizes by splitting up the line
                            var sizes = output.Select(f => f.Split(' ').First()).Where(f => f.All(char.IsDigit)).Sum(int.Parse);
                            
                            paths.TryAdd(path, sizes); // Add the data for the 

                            break;
                    }
                }
            }

            // Sum Up
            foreach (var path1 in paths)
            {
                var subPaths = paths.Where(path2 => path1.Key != path2.Key).Where(path2 => path2.Key.StartsWith(path1.Key));
                foreach (var path2 in subPaths)
                {
                    paths[path1.Key] += path2.Value;
                }
            }

            // ASSERT

            // part 1 
            var part1 = paths.Where(f => f.Value <= 100000).Sum(f => f.Value);

            // part 2 
            var usedSpace = paths["C:\\"]; // Using dictionary forces it to assume C: not \ but thats a system thing
            var calculatedSpace = neededSpace - (totalSpaceAvailable - usedSpace);
            var part2 = paths.Where(f => f.Value >= calculatedSpace).Min(f => f.Value);
        }
    }
}