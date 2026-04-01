using System.Collections.Generic;

namespace Update_Tag
{
    public interface IGit
    {
        List<Tag> GetTags();
        void NewTag(string newTag, string revision);
        void DeleteTag(string tag);
        void DeleteRemoteTag(string tag);
        void Push();
        void PushTag(string tag);
        string GetTagInfo(string revision);
        int GetCommitsBehind(string revision);
        string GetTopCommitInfo();
    }
}