﻿using System;
using Newtonsoft.Json;

namespace ZendeskApi.Client.Models
{
    public class SearchResult
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }
}
