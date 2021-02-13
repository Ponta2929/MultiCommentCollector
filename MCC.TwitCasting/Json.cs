using System;
using System.Text.Json.Serialization;

namespace MCC.TwitCasting
{
    public class Comment
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("author")]
        public User Author { get; set; }

        [JsonPropertyName("numComments")]
        public int Comments { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("screenName")]
        public string ScreenName { get; set; }

        [JsonPropertyName("profileImage")]
        public string ProfileImage { get; set; }

        [JsonPropertyName("grade")]
        public int Grade { get; set; }
    }

    public class LatestMovie
    {
        [JsonPropertyName("update_interval_sec")]
        public int UpdateItervalSecond { get; set; }

        [JsonPropertyName("movie")]
        public Movie Movie { get; set; }
    }

    public class Movie
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("is_on_live")]
        public bool IsOnLive { get; set; }
    }

    public class EventPubSubURL
    {
        [JsonPropertyName("url")]
        public string URL { get; set; }
    }
}
