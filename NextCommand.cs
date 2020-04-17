using System;
using System.Linq;

namespace Update_Tag
{
    internal class NextCommand : ICommand
    {
        private readonly string _label;
        private readonly Place _place;
        private readonly bool _dryRun;

        public NextCommand(string label, Place place, bool dryRun)
        {
            _label = label;
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

            git.NewTag(newTag.ToString());
            git.PushAll();
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