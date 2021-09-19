using System;
using System.Xml.Serialization;

namespace MCC.Youtube
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class Feed
    {
        [XmlElement("link")]
        public Link[] Link { get; set; }

        [XmlElement("id")]
        public string ID { get; set; }

        [XmlElement("channelId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string YoutubeChannelID { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("author")]
        public Author Author { get; set; }

        [XmlElement("published")]
        public DateTime Published { get; set; }

        [XmlElement("entry")]
        public Entry[] Entry { get; set; }
    }

    public class Link
    {
        [XmlAttribute("rel")]
        public string rel { get; set; }

        [XmlAttribute("href")]
        public string href { get; set; }
    }

    public class Author
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("uri")]
        public string URI { get; set; }
    }

    public class Entry
    {
        [XmlElement("id")]
        public string ID { get; set; }

        [XmlElement("videoId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string VideoID { get; set; }

        [XmlElement("channelId", Namespace = "http://www.youtube.com/xml/schemas/2015")]
        public string ChannelID { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public Link Link { get; set; }

        [XmlElement("author")]
        public Author Author { get; set; }

        [XmlElement("published")]
        public DateTime Published { get; set; }

        [XmlElement("updated")]
        public DateTime Updated { get; set; }

        [XmlElement("group", Namespace = "http://search.yahoo.com/mrss/")]
        public Group Group { get; set; }
    }

    [XmlRoot("group", Namespace = "http://search.yahoo.com/mrss/")]
    public class Group
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("content")]
        public Content Content { get; set; }

        [XmlElement("thumbnail")]
        public Thumbnail Thumbnail { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("community")]
        public Community Community { get; set; }
    }

    [XmlRoot("content", Namespace = "http://search.yahoo.com/mrss/")]
    public class Content
    {
        [XmlAttribute("url")]
        public string URL { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("width")]
        public string Width { get; set; }

        [XmlAttribute("height")]
        public string Height { get; set; }
    }

    [XmlRoot("thumbnail", Namespace = "http://search.yahoo.com/mrss/")]
    public class Thumbnail
    {
        [XmlAttribute("url")]
        public string URL { get; set; }

        [XmlAttribute("width")]
        public string Width { get; set; }

        [XmlAttribute("height")]
        public string Height { get; set; }
    }

    [XmlRoot("community", Namespace = "http://search.yahoo.com/mrss/")]
    public class Community
    {
        [XmlElement("starRating")]
        public StarRating StarRating { get; set; }

        [XmlElement("statistics")]
        public Statistics Statistics { get; set; }
    }

    [XmlRoot("starRating", Namespace = "http://search.yahoo.com/mrss/")]
    public class StarRating
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlAttribute("average")]
        public double Average { get; set; }

        [XmlAttribute("min")]
        public string Min { get; set; }

        [XmlAttribute("max")]
        public string Max { get; set; }
    }

    [XmlRoot("statistics", Namespace = "http://search.yahoo.com/mrss/")]
    public class Statistics
    {
        [XmlAttribute("views")]
        public int Views { get; set; }
    }
}