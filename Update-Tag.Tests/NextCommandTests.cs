using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Update_Tag.Tests
{
    public class NextCommandTests
    {
        [Theory]
        [InlineData(Place.Major, "1.0.0")]
        [InlineData(Place.Minor, "0.1.0")]
        [InlineData(Place.Patch, "0.0.1")]
        public void Creates_the_correct_first_tag(Place place, string expectedTag)
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>());
            
            var nextMajorVersionCommand = new NextCommand(place, git.Object);
            
            nextMajorVersionCommand.Execute(Array.Empty<string>());
            
            git.Verify(x => x.NewTag(expectedTag, null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag(expectedTag));
        }
        
        [Fact]
        public void Creates_the_correct_first_tag_for_a_specific_label()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>());
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "rc" });
            
            git.Verify(x => x.NewTag("0.0.1-RC.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("0.0.1-RC.1"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_major_version_when_there_is_another_major_tag_present()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Major, git.Object);
            
            nextMajorVersionCommand.Execute(Array.Empty<string>());
            
            git.Verify(x => x.NewTag("2.0.0", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("2.0.0"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_minor_version_when_there_is_a_major_tag_present()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Minor, git.Object);
            
            nextMajorVersionCommand.Execute(Array.Empty<string>());
            
            git.Verify(x => x.NewTag("1.1.0", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.1.0"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_patch_version_when_there_is_a_major_tag_present()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Patch, git.Object);
            
            nextMajorVersionCommand.Execute(Array.Empty<string>());
            
            git.Verify(x => x.NewTag("1.0.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.0.1"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_patch_version_when_there_is_a_major_and_minor_tag_present()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(1, 1, 0, null, null)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Patch, git.Object);
            
            nextMajorVersionCommand.Execute(Array.Empty<string>());
            
            git.Verify(x => x.NewTag("1.1.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.1.1"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_label_when_a_major_tag_is_the_latest_tag()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(0, 1, 0, null, null),
                new Tag(0, 1, 0, "RC", 1)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "rc" });
            
            git.Verify(x => x.NewTag("1.1.0-RC.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.1.0-RC.1"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_label_when_a_minor_tag_is_the_latest_tag()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(1, 1, 0, null, null),
                new Tag(0, 1, 0, "RC", 1)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "rc" });
            
            git.Verify(x => x.NewTag("1.2.0-RC.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.2.0-RC.1"));
        }
        
        [Fact]
        public void Creates_the_correct_tag_for_a_label_when_a_major_tag_is_the_latest_tag_and_a_new_major_version_should_be_created()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(0, 1, 0, null, null),
                new Tag(0, 1, 0, "RC", 1)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "rc", "-X" });
            
            git.Verify(x => x.NewTag("2.0.0-RC.1", null));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("2.0.0-RC.1"));
        }

        [Fact]
        public void Creates_a_tag_for_a_given_revision()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(0, 1, 0, null, null),
                new Tag(0, 1, 0, "RC", 1)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "test", "0.1.0" });
            
            git.Verify(x => x.NewTag("1.1.0-TEST.1", "0.1.0"));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.1.0-TEST.1"));
        }
        
        [Fact]
        public void Creates_a_tag_for_a_given_label()
        {
            var git = new Mock<IGit>();
            git.Setup(x => x.GetTags()).Returns(new List<Tag>
            {
                new Tag(1, 0, 0, null, null),
                new Tag(0, 1, 0, null, null),
                new Tag(0, 1, 0, "RC", 1)
            });
            
            var nextMajorVersionCommand = new NextCommand(Place.Version, git.Object);
            
            nextMajorVersionCommand.Execute(new []{ "test", "rc" });
            
            git.Verify(x => x.NewTag("1.1.0-TEST.1", "0.1.0-RC.1"));
            git.Verify(x => x.Push());
            git.Verify(x => x.PushTag("1.1.0-TEST.1"));
        }
    }
}