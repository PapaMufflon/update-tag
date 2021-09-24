using System;

namespace Update_Tag
{
    public class Tag
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string Label { get; set; }
        public int? Version { get; set; }

        public Tag(int major, int minor, int patch, string label, int? version)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Label = string.IsNullOrEmpty(label) ? string.Empty : label.ToUpperInvariant();
            Version = string.IsNullOrEmpty(label) ? null : version;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Label)
                ? $"{Major}.{Minor}.{Patch}"
                : $"{Major}.{Minor}.{Patch}-{Label}.{Version}";
        }

        public Tag Increment(Place place, string label, bool newMajorVersion)
        {
            if (place == Place.Version &&
                !string.IsNullOrEmpty(label) &&
                string.IsNullOrEmpty(Label))
            {
                return IncrementWithLabel(label, newMajorVersion);
            }
            
            return place switch
            {
                Place.Major => new Tag(Major + 1, 0, 0, Label, string.IsNullOrEmpty(Label) ? (int?) null : 1),
                Place.Minor => new Tag(Major, Minor + 1, 0, Label, string.IsNullOrEmpty(Label) ? (int?) null : 1),
                Place.Patch => new Tag(Major, Minor, Patch + 1, Label, string.IsNullOrEmpty(Label) ? (int?) null : 1),
                Place.Version => new Tag(Major, Minor, Patch, Label, Version + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(place), place, null)
            };
        }

        private Tag IncrementWithLabel(string label, bool newMajorVersion)
        {
            return newMajorVersion
                ? new Tag(Major + 1, 0, 0, label, 1)
                : new Tag(Major, Minor + 1, 0, label, 1);
        }
    }
}