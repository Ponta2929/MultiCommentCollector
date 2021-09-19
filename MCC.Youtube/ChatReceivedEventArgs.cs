using System;

namespace MCC.Youtube
{
    public delegate void ChatReceivedEventHandler(object sender, ChatReceivedEventArgs e);

    public class ChatReceivedEventArgs : EventArgs
    {
        public ChatReceivedEventArgs(VideoListResponse data) => ReceiveData = data;

        public VideoListResponse ReceiveData { get; private set; }
    }
}
