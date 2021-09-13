using MCC.Utility;

namespace MCC.Plugin
{
    public interface IPluginReceiver : IPluginBase
    {
        void Receive(CommentData comment);
    }
}
