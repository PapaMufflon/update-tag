using System;
using System.Linq;

namespace Update_Tag
{
    internal class NextCommand : ICommand
    {
        private readonly string _label;
        private readonly string _revision;
        private readonly Place _place;
        private readonly bool _dryRun;

        public NextCommand(string[] arguments, Place place, bool dryRun)
        {
            if (place == Place.Version)
            {
                _label = arguments.FirstOrDefault(x =>
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d"));
                
                _revision = arguments.FirstOrDefault(x =>
                    !x.Equals(_label) &&
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d"));
            }
            else
            {
                _revision = arguments.FirstOrDefault(x =>
                    !x.Equals("--dry-run") &&
                    !x.Equals("-d"));
            }
            
            _place = place;
            _dryRun = dryRun;
        }

        public void Execute()
        {
            if (_place == Place.Version && string.IsNullOrEmpty(_label))
            {
                throw new Exception("No label given, cannot increment Version");
            }
            
            var git = new Git(_dryRun);

            var allTags = git.GetTags();
            
            var specificTags = string.IsNullOrEmpty(_label)
                ? allTags.Where(x => string.IsNullOrEmpty(x.Label))
                : allTags.Where(x => x.Label.ToLowerInvariant().Equals(_label));

            var latestTag = specificTags
                .OrderBy(x => x.Major)
                .ThenBy(x => x.Minor)
                .ThenBy(x => x.Patch)
                .ThenBy(x => x.Version)
                .LastOrDefault();

            var newTag = latestTag == null
                ? GetNewTag()
                : latestTag.Increment(_place);

            git.NewTag(newTag.ToString(), _revision);
            git.Push();
            git.PushTag(newTag.ToString());
        }

        private Tag GetNewTag()
        {
            return _place switch
            {
                Place.Major => new Tag(1, 0, 0, _label, 1),
                Place.Minor => new Tag(0, 1, 0, _label, 1),
                Place.Patch => new Tag(0, 0, 1, _label, 1),
                Place.Version => new Tag(0, 0, 1, _label, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}