using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keove.Dto.Responses
{
    public class ListResponseDto<Tdto>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<Tdto> Data { get; set; }
    }
}
