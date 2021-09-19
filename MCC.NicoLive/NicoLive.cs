using MCC.Plugin;
using MCC.Utility;
using MCC.Utility.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCC.NicoLive
{
    public class NicoLive : IPluginSender, ILogged
    {
        private NicoLiveConnector connector = new();
        private Hashtable table = new();

        public string Author => "ぽんた";

        public string PluginName => "ニコニコ生放送";

        public string StreamKey { get; set; }

        public string Description => "ニコニコ生放送の配信中のコメントを取得します。";

        public string Version => "1.0.0";

        public string SiteName => "NicoLive";

        public event CommentReceivedEventHandler OnCommentReceived;
        public event LoggedEventHandler OnLogged;

        public bool Activate()
        {
            connector.Resume = true;

            Task.Run(() => connector.Connect(StreamKey));

            return true;
        }

        public bool Inactivate()
        {
            connector.Resume = false;

            connector.Abort();

            return true;
        }

        public bool IsSupport(string url)
        {
            var livePage = url.RegexString(@"https://live[\d]*.nicovideo.jp/watch/(?<value>[\w]+)", "value");
            var communityPage = url.RegexString(@"https://com.nicovideo.jp/community/(?<value>[\w]+)", "value");

            StreamKey = !string.IsNullOrEmpty(livePage) ? livePage : communityPage;

            if (!string.IsNullOrEmpty(livePage) || !string.IsNullOrEmpty(communityPage))
            {
                return true;
            }

            return false;
        }

        public void PluginClose() => Inactivate();

        public void PluginLoad()
        {
            connector.OnLogged += BaseLogged;
            connector.OnReceived += OnReceived;

            table["Emotion"] = new AdditionalData() { Data = "Emotion", Description = "エモーション" };
            table["Request"] = new AdditionalData() { Data = "Request", Description = "放送ネタ" };
            table["Gift"] = new AdditionalData() { Data = "Gift", Description = "ギフト" };
            table["184"] = new AdditionalData() { Data = "184", Description = "184" };
            table["Ad"] = new AdditionalData() { Data = "Ad", Description = "ニコニコ広告" };
            table["P"] = new AdditionalData() { Data = "P", Description = "プレミアム会員" };
            table["運"] = new AdditionalData() { Data = "運", Description = "運営コメント" };
        }

        private void OnReceived(object sender, ChatReceivedEventArgs e)
        {
            var chat = e.ReceiveData.Chat;
            var list = new List<AdditionalData>();

            var comment = new CommentData()
            {
                LiveName = "NicoLive",
                PostType = PostType.Comment,
                UserID = e.ReceiveData.Chat.UserId,
                PostTime = e.ReceiveData.Chat.Date.LocalDateTime,
                UserName = string.Empty,
            };

            if (chat.Premium == 3 && chat.Content.StartsWith("/emotion"))
            {
                list.Add(table["Emotion"] as AdditionalData);
                comment.Comment = e.ReceiveData.Chat.Content.Replace("/emotion ", null);
            }
            else if (chat.Premium == 3 && chat.Content.StartsWith("/spi"))
            {
                list.Add(table["Request"] as AdditionalData);
                comment.Comment = e.ReceiveData.Chat.Content.Replace("/spi ", null).Replace("\"", null);
            }
            else if (chat.Premium == 3 && chat.Content.StartsWith("/gift"))
            {
                list.Add(table["Gift"] as AdditionalData);
                comment.Comment = e.ReceiveData.Chat.Content.Split(" ")[6].Replace("\"", null);
            }
            else if (chat.Premium == 3 && chat.Content.StartsWith("/nicoad"))
            {
                var replace = chat.Content.Replace("/nicoad ", null);
                var ad = JsonSerializer.Deserialize<NicoAd>(replace);
                list.Add(table["Ad"] as AdditionalData);
                comment.Comment = ad.Message;
            }
            else
            {
                if (e.ReceiveData.Chat.Premium == 1)
                    list.Add(table["P"] as AdditionalData);
                if (e.ReceiveData.Chat.Anonymity)
                    list.Add(table["184"] as AdditionalData);
                if (e.ReceiveData.Chat.Premium == 3)
                    list.Add(table["運"] as AdditionalData);

                if (chat.Premium == 3 && chat.Content.StartsWith("/info 3"))
                {
                    comment.Comment = e.ReceiveData.Chat.Content.Replace("/info 3 ", null);
                }
                else if (chat.Premium == 3 && chat.Content.StartsWith("/info 10"))
                {
                    comment.Comment = e.ReceiveData.Chat.Content.Replace("/info 10 ", null);
                }
                else if (chat.Premium == 3 && chat.Content.StartsWith("/perm"))
                {
                    comment.Comment = ConvertHTMLCode(e.ReceiveData.Chat.Content.Replace("/perm ", null));
                }
                else
                {
                    comment.Comment = ConvertHTMLCode(e.ReceiveData.Chat.Content);
                }
            }

            // 追加情報
            comment.Additional = list.ToArray();

            OnCommentReceived?.Invoke(this, new(comment));
        }

        private void BaseLogged(object sender, LoggedEventArgs e) => OnLogged?.Invoke(this, e);

        private string ConvertHTMLCode(string message)
        {
            var reg = @"<a\s+[^>]*href\s*=\s*[""'](?<href>[^""']*)[""'][^>]*>(?<text>[^<]*)</a>";
            var r = new Regex(reg, RegexOptions.IgnoreCase);
            var collection = r.Matches(message);

            foreach (Match m in collection)
            {
                if (m.Success)
                {
                    return m.Groups["text"].Value.Trim();
                }
            }

            return message;
        }
    }
}
