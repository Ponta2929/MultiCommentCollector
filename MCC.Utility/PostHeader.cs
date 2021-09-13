using MCC.Utility.Binding;

namespace MCC.Utility
{
    public class PostHeader : BindableBase
    {
        public PostType PostType { get; set; }
    }

    public enum PostType
    {
        Comment = 0,
        UserSetting = 1,
    }
}
