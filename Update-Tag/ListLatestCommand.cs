using System;
using System.Linq;

namespace Update_Tag
{
    internal class ListLatestCommand : ICommand
    {
        private readonly IGit _git;

        public ListLatestCommand(IGit git)
        {
            _git = git;
        }
        
        public void Execute(string[] arguments)
        {
            var topCommit = _git.GetTopCommitInfo();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"latest ({topCommit.Trim()})");
            Console.WriteLine();
            
            var allTags = _git.GetTags();
            var allLatestTags = allTags
                .OrderBy(x => x.Major)
                .ThenBy(x => x.Minor)
                .ThenBy(x => x.Patch)
                .ThenBy(x => x.Version)
                .GroupBy(x => x.Label)
                .Select(x => x.Last())
                .Select(x => (x, _git.GetCommitsBehind(x.ToString())))
                .OrderBy(tuple => tuple.x.Label)
                .ThenBy(tuple => tuple.Item2)
                .Select(tuple => tuple.x)
                .ToList();

            allLatestTags.Reverse();
            
            allLatestTags.ForEach(x =>
                {
                    var tagInfo = _git.GetTagInfo(x.ToString());

                    Console.ForegroundColor = tagInfo.StartsWith(topCommit)
                        ? ConsoleColor.Green
                        : ConsoleColor.DarkYellow;
                    
                    Console.WriteLine($"{x} ({tagInfo.Trim()})");
                });
            
            Console.ResetColor();
        }
    }
}