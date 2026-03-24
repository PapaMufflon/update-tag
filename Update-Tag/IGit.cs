using System.Collections.Generic;

namespace Update_Tag
{
    public interface IGit
    {
        List<Tag> GetTags();
        void NewTag(string newTag, string revision);
        void Push();
        void PushTag(string tag);
        string GetTagInfo(string revision);
        int GetCommitsBehind(string revision);
        string GetTopCommitInfo();

        List<string> GetRecentMergedHeadTags(int count);
        void DeleteLocalTags(IEnumerable<string> tags);
        void DeleteRemoteTags(IEnumerable<string> tags);
    }
}