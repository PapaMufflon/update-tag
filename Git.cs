using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Update_Tag
{
    public class Git
    {
        private readonly bool _dryRun;

        public Git(bool dryRun)
        {
            _dryRun = dryRun;
        }

        public List<Tag> GetTags()
        {
            var tagsResponse = ExecuteCommand("git", "--no-pager tag");

            if (tagsResponse.StartsWith("fatal: not a git repository (or any of the parent directories):"))
            {
                throw new Exception(tagsResponse);
            }

            return Parse(tagsResponse);
        }
        
        public void NewTag(string newTag, string revision)
        {
            Console.WriteLine($"git tag {newTag} {revision}");

            if (_dryRun)
                return;
            
            var response = ExecuteCommand("git", $"tag {newTag} {revision}");

            if (!string.IsNullOrEmpty(response))
                Console.WriteLine(response);
        }
        
        public void Push()
        {
            Console.WriteLine("git push");
            
            if (_dryRun)
                return;
            
            var response = ExecuteCommand("git", "push");
            
            if (!string.IsNullOrEmpty(response))
                Console.WriteLine(response);
        }
        
        public void PushTag(string tag)
        {
            Console.WriteLine($"git push origin {tag}");
            
            if (_dryRun)
                return;
            
            var response = ExecuteCommand("git", $"push origin {tag}");
            
            if (!string.IsNullOrEmpty(response))
                Console.WriteLine(response);
        }

        public string GetTagInfo(string revision)
        {
            return ExecuteCommand("git", $"log -1 {revision} --format=\"(%s, %ar)\"");
        }
        
        public string GetTopCommitInfo()
        {
            return ExecuteCommand("git", $"log -1 --format=\"(%s, %ar)\"");
        }
        
        private static List<Tag> Parse(string tagsResponse)
        {
            var regex = @"^(?<major>\d*)\.(?<minor>\d*)\.(?<patch>\d*)(-(?<label>[a-zA-Z-]*)(\.(?<version>\d*)))?$";
            var singleTags = tagsResponse.Split(new []{Environment.NewLine, "\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            return singleTags
                .Select(x =>
                {
                    var match = Regex.Match(x, regex);

                    if (!match.Success)
                        return null;

                    return new Tag(
                        int.Parse(match.Groups["major"].Value),
                        int.Parse(match.Groups["minor"].Value),
                        int.Parse(match.Groups["patch"].Value),
                        match.Groups["label"].Value,
                        string.IsNullOrEmpty(match.Groups["version"].Value) ? (int?)null : int.Parse(match.Groups["version"].Value));
                })
                .Where(x => x != null)
                .ToList();
        }
        
        private string ExecuteCommand(string command, string arguments)
        {
            var result = new StringBuilder();
            
            using var proc = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();

            result.Append(proc.StandardOutput.ReadToEnd());
            result.Append(proc.StandardError.ReadToEnd());

            proc.WaitForExit();

            return result.ToString();
        }
    }
}