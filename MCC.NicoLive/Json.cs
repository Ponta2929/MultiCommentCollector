﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    public class ReceiveChat {

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

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("premium")]
        public int Premium { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
