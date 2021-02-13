using System;

namespace MCC.Utility
{
    public delegate void CommentReceivedEventHandler(object sender, CommentReceivedEventArgs e);

    public class CommentReceivedEventArgs : EventArgs
    {
        public CommentReceivedEventArgs(CommentData data)
          => CommentData = data;

        public CommentData CommentData { get; private set; }
    }
}
