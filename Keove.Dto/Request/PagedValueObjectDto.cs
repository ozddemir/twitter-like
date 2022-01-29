using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keove.Dto.Request
{
   public class PagedValueObjectDto
    {
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("count")]
        public int? Count { get; set; }
        public PagedValueObjectDto()
        {

        }
    }
}
