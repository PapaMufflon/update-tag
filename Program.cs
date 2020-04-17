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
                command.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static ICommand GetCommand(string[] args)
        {
            if (args.Length == 0 || args.Length > 3 || args[0].Equals("--help"))
            {
                WriteUsage();
                return null;
            }

            var dryRun = args
                .Skip(1)
                .Any(x => x.Equals("--dry-run") ||
                          x.Equals("-d"));

            if (dryRun)
                Console.WriteLine("dry run, commands won't be executed" + Environment.NewLine);

            var label = dryRun
                ? args.Length == 2
                    ? null
                    : args[2]
                : args.SecondOrDefault();
            
            var place = args[0] switch
            {
                var x when x == "next" || x == "n" => Place.Version,
                var x when x == "nextPatch" || x == "np" => Place.Patch,
                var x when x == "nextMinor" || x == "nx" => Place.Minor,
                var x when x == "nextMajor" || x == "nX" => Place.Major,
                _ => throw new Exception("Unknown command: " + args[0])
            };

            return new NextCommand(label, place, dryRun);
        }

        private static void WriteUsage()
        {
            Console.WriteLine(@"usage: update-tag command [label] [options]

available commands:
next (n): creates a new tag by incrementing the version number
nextPatch (np): creates a new tag by incrementing the patch number
nextMinor (nx): creates a new tag by incrementing the minor number
nextMajor (nX): creates a new tag by incrementing the major number

label: the optional label to increment

options:
--dry-run (-d): only executes non-mutating  git commands like listing tags, prints the others to console instead");
        }
    }
}