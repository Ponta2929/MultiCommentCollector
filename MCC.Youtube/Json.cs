﻿using System;

namespace MCC.Youtube
{
    public class VideoListResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Item[] items { get; set; }
        public Pageinfo pageInfo { get; set; }
        public int pollingIntervalMillis { get; set; }
        public string nextPageToken { get; set; }
    }

    public class Pageinfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public Snippet snippet { get; set; }
        public Livestreamingdetails liveStreamingDetails { get; set; }
        public Authordetails authorDetails { get; set; }
    }

    public class Snippet
    {
        public DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string[] tags { get; set; }
        public string categoryId { get; set; }
        public string liveBroadcastContent { get; set; }
        public Localized localized { get; set; }
        public string defaultAudioLanguage { get; set; }
        public string type { get; set; }
        public string liveChatId { get; set; }
        public string authorChannelId { get; set; }
        public bool hasDisplayContent { get; set; }
        public string displayMessage { get; set; }
        public Textmessagedetails textMessageDetails { get; set; }
        public SuperChatDetails superChatDetails { get; set; }
    }

    public class SuperChatDetails
    {

        public string amountMicros { get; set; }

        public string currency { get; set; }

        public string amountDisplayString { get; set; }

        public string userComment { get; set; }

        public int tier { get; set; }
    }

    public class Thumbnails
    {
        public Default _default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
        public Standard standard { get; set; }
        public Maxres maxres { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class High
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Standard
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Maxres
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Localized
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class Livestreamingdetails
    {
        public DateTime actualStartTime { get; set; }
        public DateTime scheduledStartTime { get; set; }
        public DateTime scheduledEndTime { get; set; }
        public string concurrentViewers { get; set; }

        public string activeLiveChatId { get; set; }
    }

    public class Textmessagedetails
    {
        public string messageText { get; set; }
    }

    public class Authordetails
    {
        public string channelId { get; set; }
        public string channelUrl { get; set; }
        public string displayName { get; set; }
        public string profileImageUrl { get; set; }
        public bool isVerified { get; set; }
        public bool isChatOwner { get; set; }
        public bool isChatSponsor { get; set; }
        public bool isChatModerator { get; set; }
    }
}
