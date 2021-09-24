using System;
using System.Linq;

namespace Update_Tag
{
    class Program
    {
        public static void Main(string[] args)
        {
            var command = GetCommand(args);

            if (command == null)
                return;
            
            try
            {
                command.Execute(args.Skip(1).ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static ICommand GetCommand(string[] args)
        {
            if (args.Length == 0 || args.Length > 4 || args[0].Equals("--help"))
            {
                WriteUsage();
                return null;
            }

            var dryRun = args
                .Skip(1)
                .Any(x =>
                    x.Equals("--dry-run") ||
                    x.Equals("-d"));

            if (dryRun)
                Console.WriteLine("dry run, commands won't be executed" + Environment.NewLine);

            var git = new Git(dryRun);
            
            return args[0] switch
            {
                var x when x == "next" || x == "n" => new NextCommand(Place.Version, git),
                var x when x == "nextPatch" || x == "np" => new NextCommand(Place.Patch, git),
                var x when x == "nextMinor" || x == "nx" => new NextCommand(Place.Minor, git),
                var x when x == "nextMajor" || x == "nX" => new NextCommand(Place.Major, git),
                var x when x == "listLatest" || x == "l" => new ListLatestCommand(git),
                _ => throw new Exception("Unknown command: " + args[0])
            };
        }

        private static void WriteUsage()
        {
            Console.WriteLine(@"usage: update-tag command [label] [revision | -X] [options]

available commands:
next (n): creates a new tag by incrementing the version number
nextPatch (np): creates a new tag by incrementing the patch number
nextMinor (nx): creates a new tag by incrementing the minor number
nextMajor (nX): creates a new tag by incrementing the major number
listLatest (l): lists latest tag versions in each category, label and dry-run gets ignored

label: the optional label to increment
revision: the optional revision (or tag) to add the new tag to
--next-major-version (-X): only available for `update-tag n [label]`; creates a new major version for the label

options:
--dry-run (-d): only executes non-mutating git commands like listing tags, prints the others to console instead

examples:
update-tag np                creates a new tag incrementing the patch number (0.0.1 if it's the first one)
update-tag n test            creates a new tag incrementing the version number (0.0.1-TEST.1 if it's the first one)
update-tag nX 9ba4fa1        creates a new tag incrementing the major number for the revision 9ba4fa1
update-tag nx 0.1.3-TEST.3   creates a new tag incrementing the minor number for the revision where there is also tag 0.1.3-TEST.3
update-tag np test           creates a new tag incrementing the patch number for the revision with the latest test tag
update-tag n test rc         creates a new tag incrementing the version number for the revision with the latest rc tag");
        }
    }
}