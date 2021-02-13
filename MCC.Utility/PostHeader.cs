namespace MCC.Utility
{
    public class PostHeader
    {
        public PostType PostType { get; set; }
    }

    public enum PostType
    {
        Comment = 0,
        UserSetting = 1,
    }
}
