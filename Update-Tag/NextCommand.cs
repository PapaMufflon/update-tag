using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Update_Tag
{
    public class NextCommand : ICommand
    {
        private readonly Place _place;
        private readonly IGit _git;

        public NextCommand(Place place, IGit git)
        {
            _place = place;
            _git = git;
        }

        public void Execute(string[] arguments)
        {
            string revision;
            string label = null;
            
            var newMajorVersion = arguments.Any(x =>
                x == "-X" ||
                x == "--next-major-version");
            
            if (_place == Place.Version)
            {
                label = arguments.FirstOrDefault(x =>
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d") &&
                    !x.Equals("--next-major-version") &&
                    !x.Equals("-X"));
                
                revision = arguments.FirstOrDefault(x =>
                    !x.Equals(label) &&
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d") &&
                    !x.Equals("--next-major-version") &&
                    !x.Equals("-X"));
            }
            else
            {
                revision = arguments.FirstOrDefault(x =>
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d"));
            }
            
            if (_place == Place.Version && string.IsNullOrEmpty(label))
            {
                throw new Exception("No label given, cannot increment Version");
            }

            var allTags = _git.GetTags();
            Tag revisionTag = null;
            
            if (!string.IsNullOrEmpty(revision))
            {
                revisionTag = allTags.SingleOrDefault(x =>
                    x.ToString().Equals(revision) ||
                    (x.Label?.Equals(revision, StringComparison.InvariantCultureIgnoreCase)).GetValueOrDefault(false));
                
                if (revisionTag == null)
                {
                    throw new Exception("Cannot find a commit with revision or label " + revision);
                }
            }
            
            var specificTags = string.IsNullOrEmpty(label)
                ? allTags.Where(x => string.IsNullOrEmpty(x.Label))
                : allTags.Where(x => x.Label.ToLowerInvariant().Equals(label) || string.IsNullOrEmpty(x.Label));

            var latestTag = specificTags
                .OrderBy(x => x.Major)
                .ThenBy(x => x.Minor)
                .ThenBy(x => x.Patch)
                .ThenBy(x => x.Version)
                .LastOrDefault();

            var newTag = latestTag == null
                ? GetNewTag(label)
                : latestTag.Increment(_place, label, newMajorVersion);

            _git.NewTag(newTag.ToString(), revisionTag?.ToString());
            _git.Push();
            _git.PushTag(newTag.ToString());
        }

        private Tag GetNewTag(string label)
        {
            return _place switch
            {
                Place.Major => new Tag(1, 0, 0, label, 1),
                Place.Minor => new Tag(0, 1, 0, label, 1),
                Place.Patch => new Tag(0, 0, 1, label, 1),
                Place.Version => new Tag(0, 0, 1, label, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}