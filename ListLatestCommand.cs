using System;
using System.Linq;

namespace Update_Tag
{
    internal class ListLatestCommand : ICommand
    {
        public void Execute()
        {
            var git = new Git(false);
            var allTags = git.GetTags();
            allTags
                .OrderBy(x => x.Major)
                .ThenBy(x => x.Minor)
                .ThenBy(x => x.Patch)
                .ThenBy(x => x.Version)
                .GroupBy(x => x.Label)
                .Select(x => x.Last())
                .ToList()
                .ForEach(x => Console.WriteLine($"{x} {git.GetTagInfo(x.ToString()).Trim()}"));
        }
    }
}