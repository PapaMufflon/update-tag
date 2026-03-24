using System;
using System.Collections.Generic;
using System.Linq;

namespace Update_Tag
{
    public class DeleteCommand : ICommand
    {
        private const int DefaultMaxTags = 50;

        private readonly IGit _git;

        public DeleteCommand(IGit git)
        {
            _git = git;
        }

        public void Execute(string[] arguments)
        {
            var deleteLocal = arguments.Contains("--local");
            var deleteRemote = arguments.Contains("--remote");

            // Default: delete both local and remote tags.
            // If exactly one flag is present, delete only that scope.
            if (!deleteLocal && !deleteRemote)
            {
                deleteLocal = true;
                deleteRemote = true;
            }

            var tags = _git.GetRecentMergedHeadTags(DefaultMaxTags);

            if (tags.Count == 0)
            {
                Console.WriteLine("No tags found.");
                return;
            }

            Console.WriteLine("Recent tags:");
            Console.WriteLine();

            for (var i = 0; i < tags.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tags[i]}");
            }

            Console.WriteLine();
            Console.Write("Enter indexes to delete (e.g. 1,3,7 or 1-7), empty to cancel: ");
            var selectionInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(selectionInput))
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            HashSet<int> selectedIndexes;
            try
            {
                selectedIndexes = ParseSelection(selectionInput, tags.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid selection: " + e.Message);
                return;
            }

            var selectedTags = selectedIndexes
                .OrderBy(i => i)
                .Select(i => tags[i - 1])
                .ToList();

            if (selectedTags.Count == 0)
            {
                Console.WriteLine("No tags selected.");
                return;
            }

            Console.WriteLine();
            Console.Write($"Delete {selectedTags.Count} tags? [y/N]: ");
            var confirm = Console.ReadLine();

            if (!string.Equals(confirm, "y", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Aborted.");
                return;
            }

            if (deleteLocal)
            {
                _git.DeleteLocalTags(selectedTags);
            }

            if (deleteRemote)
            {
                _git.DeleteRemoteTags(selectedTags);
            }
        }

        // Input examples:
        // - "1,3,7"
        // - "1-7"
        // - "2-4,1"
        public static HashSet<int> ParseSelection(string input, int maxIndex)
        {
            if (maxIndex < 1)
                throw new ArgumentException("maxIndex must be >= 1");

            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Selection must not be empty");

            var selection = new HashSet<int>();

            var tokens = input
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t));

            foreach (var token in tokens)
            {
                if (token.Contains("-"))
                {
                    var rangeParts = token
                        .Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                    if (rangeParts.Length != 2)
                        throw new ArgumentException($"Invalid range: '{token}'");

                    if (!int.TryParse(rangeParts[0].Trim(), out var start))
                        throw new ArgumentException($"Invalid range start in: '{token}'");

                    if (!int.TryParse(rangeParts[1].Trim(), out var end))
                        throw new ArgumentException($"Invalid range end in: '{token}'");

                    if (start > end)
                        throw new ArgumentException($"Invalid range order in: '{token}'");

                    if (start < 1 || end > maxIndex)
                        throw new ArgumentException($"Range '{token}' is outside 1..{maxIndex}");

                    for (var i = start; i <= end; i++)
                    {
                        selection.Add(i);
                    }
                }
                else
                {
                    if (!int.TryParse(token, out var index))
                        throw new ArgumentException($"Invalid index: '{token}'");

                    if (index < 1 || index > maxIndex)
                        throw new ArgumentException($"Index '{index}' is outside 1..{maxIndex}");

                    selection.Add(index);
                }
            }

            if (selection.Count == 0)
                throw new ArgumentException("No valid indexes were provided");

            return selection;
        }
    }
}

