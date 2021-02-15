using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
