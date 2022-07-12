using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTS_Downloader
{
    internal class MovieModel
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Data
        {
            [JsonProperty("movie_count")]
            public int MovieCount { get; set; }

            [JsonProperty("page_number")]
            public int PageNumber { get; set; }

            [JsonProperty("movies")]
            public List<Movie> Movies { get; set; }
        }

        public class Movie
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("year")]
            public int Year { get; set; }

            [JsonProperty("rating")]
            public double Rating { get; set; }

            [JsonProperty("genres")]
            public List<string> Genres { get; set; }

            [JsonProperty("synopsis")]
            public string Synopsis { get; set; }

            [JsonProperty("yt_trailer_code")]
            public string YtTrailerCode { get; set; }

            [JsonProperty("medium_cover_image")]
            public string MediumCoverImage { get; set; }

            [JsonProperty("torrents")]
            public List<Torrent> Torrents { get; set; }
        }

        public class Root
        {
            [JsonProperty("data")]
            public Data Data { get; set; }
        }

        public class Torrent
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("hash")]
            public string Hash { get; set; }

            [JsonProperty("quality")]
            public string Quality { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("size")]
            public string Size { get; set; }

        }




    }
}
