using System;
using System.Net;
using Newtonsoft.Json;

namespace CoreApi.Contract
{
    [Serializable]
    public class ResponseContract
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }

        [JsonProperty("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }
    }
}
