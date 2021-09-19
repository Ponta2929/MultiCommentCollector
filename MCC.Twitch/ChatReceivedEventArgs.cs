using System;

namespace MCC.Twitch
{
    public delegate void ChatReceivedEventHandler(object sender, ChatReceivedEventArgs e);

    public class ChatReceivedEventArgs : EventArgs
    {
        public ChatReceivedEventArgs(string data) => ReceiveData = data;

        public string ReceiveData { get; private set; }
    }
}
