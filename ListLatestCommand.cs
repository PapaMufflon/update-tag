using System;
using System.Linq;

namespace Update_Tag
{
    internal class ListLatestCommand : ICommand
    {
        public void Execute()
        {
            var git = new Git(false);
            
            var topCommit = git.GetTopCommitInfo();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"latest {topCommit.Trim()}");
            
            var allTags = git.GetTags();
            allTags
                .OrderBy(x => x.Major)
                .ThenBy(x => x.Minor)
                .ThenBy(x => x.Patch)
                .ThenBy(x => x.Version)
                .GroupBy(x => x.Label)
                .Select(x => x.Last())
                .ToList()
                .ForEach(x =>
                {
                    var tagInfo = git.GetTagInfo(x.ToString());

                    Console.ForegroundColor = tagInfo.Equals(topCommit)
                        ? ConsoleColor.Green
                        : ConsoleColor.DarkYellow;
                    
                    Console.WriteLine($"{x} {tagInfo.Trim()}");
                });
            
            Console.ResetColor();
        }
    }
}