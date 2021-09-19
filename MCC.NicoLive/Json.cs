using System;
using System.Text.Json.Serialization;

namespace MCC.NicoLive
{
    public class NicoLiveJson
    {
        [JsonPropertyName("site")]
        public Site Site { get; set; }
    }
    public class Site
    {
        [JsonPropertyName("relive")]
        public ReLive ReLive { get; set; }
    }
    public class ReLive
    {
        [JsonPropertyName("webSocketUrl")]
        public string WebSocketURL { get; set; }
    }
    public class Receive
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("messageServer")]
        public MessageServer MessageServer { get; set; }

        [JsonPropertyName("threadId")]
        public string ThreadId { get; set; }
    }
    public class MessageServer
    {
        [JsonPropertyName("uri")]
        public string URI { get; set; }
    }

    public class NicoAd
    {
        [JsonPropertyName("totalAdPoint")]
        public int TotalAdPoint { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public class JsonData
    {

        [JsonPropertyName("chat")]
        public Chat Chat { get; set; }
    }

    public class Chat
    {
        [JsonPropertyName("thread")]
        public string Thread { get; set; }

        [JsonPropertyName("no")]
        public int No { get; set; }

        [JsonPropertyName("vpos")]
        public int VPos { get; set; }

        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }

        [JsonPropertyName("date_usec")]
        public int DateUsec { get; set; }

        [JsonPropertyName("mail")]
        public string Mail { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("premium")]
        public int Premium { get; set; }

        [JsonPropertyName("anonymity")]
        public bool Anonymity { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
