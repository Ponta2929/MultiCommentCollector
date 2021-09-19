using System;

namespace MCC.TwitCasting
{
    public delegate void ChatReceivedEventHandler(object sender, ChatReceivedEventArgs e);

    public class ChatReceivedEventArgs : EventArgs
    {
        public ChatReceivedEventArgs(JsonData data) => ReceiveData = data;

        public JsonData ReceiveData { get; private set; }
    }
}
