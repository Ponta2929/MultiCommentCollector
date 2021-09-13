using Reactive.Bindings;
using System;

namespace MultiCommentCollectorCLI
{
    [Serializable]
    public class Server
    {
        /// <summary>
        /// CommentReceiverServerPort
        /// </summary>
        public ReactiveProperty<int> CommentReceiverServerPort { get; set; } = new(29291);

        /// <summary>
        /// CommentGeneratorServerPort
        /// </summary>
        public ReactiveProperty<int> CommentGeneratorServerPort { get; set; } = new(29292);
    }
}
