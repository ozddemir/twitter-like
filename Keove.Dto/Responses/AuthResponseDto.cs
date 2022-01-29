using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Keove.Dto.Responses
{
   public class AuthResponseDto
    {

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("errors")]
        public List<String> Errors { get; set; }
    }
}
