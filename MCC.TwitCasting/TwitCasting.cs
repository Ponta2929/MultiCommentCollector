using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Text;
using System.Threading.Tasks;

namespace MCC.TwitCasting
{
    public class TwitCasting : IPluginSender, ILogged
    {
        private TwitCastingConnector connector = new();

        public string Author => "ぽんた";

        public string PluginName => "TwitCasting";

        public string StreamKey { get; set; }

        public string Description => "TwitCastingの配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "TwitCasting";

        public event CommentReceivedEventHandler OnCommentReceived;
        public event LoggedEventHandler OnLogged;


        public bool Activate()
        {
            connector.Resume = true;

            if (!connector.WebSocketClient.Connected)
            {
                Task.Run(() => connector.Connect(StreamKey));
                Task.Run(() => connector.CheckMovieId(StreamKey));
            }

            return true;
        }

        public bool Inactivate()
        {
            connector.Resume = false;

            connector.Abort();

            return true;
        }

        public void PluginClose() => connector.Abort();

        public void PluginLoad()
        {
            connector.OnLogged += BaseLogged;
            connector.OnReceived += OnReceived;
        }

        private void OnReceived(object sender, ChatReceivedEventArgs e)
        {
            var commentData = new CommentData()
            {
                LiveName = "TwitCasting",
                PostTime = e.ReceiveData.CreatedAt.LocalDateTime,
                Comment = e.ReceiveData.Message,
                UserName = e.ReceiveData.Author.Name,
                UserID = e.ReceiveData.Author.ID
            };

            OnCommentReceived?.Invoke(this, new(commentData));
        }

        public bool IsSupport(string url)
        {
            StreamKey = url.RegexString(@"https://twitcasting.tv/(?<value>[\w]+)", "value");

            return !string.IsNullOrEmpty(StreamKey);
        }

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);
    }
}
