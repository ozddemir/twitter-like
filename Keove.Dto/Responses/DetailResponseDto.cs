using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keove.Dto.Responses
{
    public class DetailResponseDto<Tdto>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public Tdto Data { get; set; }
        
    }
}
