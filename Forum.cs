using System.Collections.Generic;

namespace MineHopWebsite
{
    public class Forum
    {
        public enum ForumType
        {
            BanAppeal,
            Announcement,
            Discussion,
            Reply
        }
        public string Title, Author, Body;
        public ForumType Type;
        public List<Forum> Replies = new List<Forum>();

        public Forum(ForumType type, string title, string author, string body)
        {
            Type = type;
            Author = author;
            Title = title;
            Body = body;
        }
    }
}